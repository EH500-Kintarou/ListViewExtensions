using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using ListViewExtensions.Models;
using ListViewExtensions.Util;
using ListViewExtensions.ViewModels.Commands;

namespace ListViewExtensions.ViewModels
{
	/// <summary>
	/// ListViewの汎用ViewModel
	/// </summary>
	/// <typeparam name="T">Model / ViewModelの型</typeparam>
	public class ListViewViewModel<T> : ListViewViewModel<T, T>
	{
		public ListViewViewModel(ISortableObservableCollection<T> source, Dispatcher dispatcher) : base(source, p => p, dispatcher) { }

		public ListViewViewModel(ISortableObservableCollection<T> source, Dispatcher dispatcher, DispatcherPriority priority) : base(source, p => p, dispatcher, priority) { }
	}

	/// <summary>
	/// ListViewの汎用ViewModel
	/// </summary>
	/// <typeparam name="TViewModel">ViewModelの型</typeparam>
	/// <typeparam name="TModel">対応するModelの型</typeparam>
	public class ListViewViewModel<TViewModel, TModel> : ReadOnlyUIObservableCollection<TViewModel, TModel>, IListViewViewModel<TViewModel>
	{
		readonly ISortableObservableCollection<TModel> Source;
		readonly Dictionary<string, string> PropertyNameDictionary;
		readonly Dictionary<string, string> PropertyNameBackDictionary;

		#region Constructor

		public ListViewViewModel(ISortableObservableCollection<TModel> source, Func<TModel, TViewModel> converter, Dispatcher dispatcher)
			: this(source, converter, new Dictionary<string, string>(), dispatcher) { }

		public ListViewViewModel(ISortableObservableCollection<TModel> source, Func<TModel, TViewModel> converter, Dispatcher dispatcher, DispatcherPriority priority)
			: this(source, converter, new Dictionary<string, string>(), dispatcher, priority) { }

		public ListViewViewModel(ISortableObservableCollection<TModel> source, Func<TModel, TViewModel> itemConverter, IReadOnlyDictionary<string, string> propertyNameDictionary, Dispatcher dispatcher)
			: this(source, itemConverter, propertyNameDictionary, dispatcher, DispatcherPriority.Normal) { }

		public ListViewViewModel(ISortableObservableCollection<TModel> source, Func<TModel, TViewModel> itemConverter, IReadOnlyDictionary<string, string> propertyNameDictionary, Dispatcher dispatcher, DispatcherPriority priority)
			: base(source, itemConverter, dispatcher, priority)
		{
			Source = source;
			Source.PropertyChanged += Source_PropertyChanged;
			this.CollectionChanged += ListViewViewModel_CollectionChanged;
			_SortingCondition = Source.SortingCondition;
			UseNonUIThreadWhenCallingModel = false;
			DisableCommandWhenCommandTaskRunning = false;
			CommandTaskRunning = false;

			PropertyNameDictionary = new Dictionary<string, string>();
			PropertyNameBackDictionary = new Dictionary<string, string>();
			foreach(var item in propertyNameDictionary) {
				var key = item.Key;
				var value = item.Value;

				PropertyNameDictionary.Add(key, value);
				PropertyNameBackDictionary.Add(value, key);
			}

			//Commandの初期化
			RemoveSelectedItemCommand = new Command(
				() => SelectedItems.Count > 0
						&& !(DisableCommandWhenCommandTaskRunning && CommandTaskRunning),
				() => SafetySourceAccessIfAvailable(() => {
					CommandTaskRunning = true;					
					if(Source is SyncedObservableCollection<TModel> syncedCollection) { //RemoveRangeの活用
						foreach(var indeces in SelectedItems.Select(p => this.IndexOf(p)).OrderBy(p => p).Distinct().Split((p, n) => p + 1 != n).Select(p => p.ToArray()).Reverse().ToArray())
							syncedCollection.RemoveRange(indeces[0], indeces.Length);
					} else {					
						foreach(var selected in SelectedItems.ToArray())
							Source.RemoveAt(this.IndexOf(selected));
					}
					CommandTaskRunning = false;
				})
			);

			MoveUpSelectedItemCommand = new Command(
				() => SelectedItems.Count > 0 && this.Count > 0 && !SelectedItems.Contains(this.First())    //SelectedItemsより先にthisの要素が消されることがある→this.First()で例外発生
						&& !(DisableCommandWhenCommandTaskRunning && CommandTaskRunning),
				() => SafetySourceAccessIfAvailable(() => {
					CommandTaskRunning = true;

					var selected = SelectedItems.Select(p => this.IndexOf(p)).OrderBy(p => p).Distinct();
					if(Source is SyncedObservableCollection<TModel> syncedCollection) { //RemoveRangeの活用
						foreach(var selectedindeces in selected.Split((p, n) => p + 1 != n).Select(p => p.ToArray()).ToArray())
							syncedCollection.MoveRange(selectedindeces[0], selectedindeces[0] - 1, selectedindeces.Length);
					} else {
						foreach(var selectedindex in selected.ToArray())
							Source.Move(selectedindex, selectedindex - 1);
					}

					CommandTaskRunning = false;
				})
			);

			MoveDownSelectedItemCommand = new Command(
				() => SelectedItems.Count > 0 && this.Count > 0 && !SelectedItems.Contains(this.Last())		//SelectedItemsより先にthisの要素が消されることがある→this.Last()で例外発生
						&& !(DisableCommandWhenCommandTaskRunning && CommandTaskRunning),
				() => SafetySourceAccessIfAvailable(() => {
					CommandTaskRunning = true;

					var selected = SelectedItems.Select(p => this.IndexOf(p)).OrderByDescending(p => p).Distinct();
					if(Source is SyncedObservableCollection<TModel> syncedCollection) { //RemoveRangeの活用
						foreach(var selectedindeces in selected.Split((p, n) => p - 1 != n).Select(p => p.Reverse().ToArray()).ToArray())
							syncedCollection.MoveRange(selectedindeces[0], selectedindeces[0] + 1, selectedindeces.Length);
					} else {
						foreach(var selectedindex in selected.ToArray())
							Source.Move(selectedindex, selectedindex + 1);
					}

					CommandTaskRunning = false;
				})
			);

			SortByPropertyCommand = new ParameterCommand<string>(
				propertyName => !(DisableCommandWhenCommandTaskRunning && CommandTaskRunning),
				propertyName => {
					if(propertyName != null)
						SafetySourceAccessIfAvailable(() => {
							CommandTaskRunning = true;
							if(SortingCondition.PropertyName == propertyName && SortingCondition.Direction == SortingDirection.Ascending)
								Source.Sort(SortingDirection.Descending, PropertyNameBackConverter(propertyName));
							else
								Source.Sort(SortingDirection.Ascending, PropertyNameBackConverter(propertyName));
							CommandTaskRunning = false;
						});
				}
			);

			ToggleSelectionCommand = new Command(() => this.Count > 0, ()=>{
				foreach(var item in this)
					ToggleItemSelection(item);
			});
		}

		private void ListViewViewModel_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			RaiseRemoveSelectedItemCanExecuteChanged();
			RaiseMoveSelectedItemCanExecuteChanged();
			RaiseToggleSelectionCanExecuteChanged();
		}

		private void Source_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName) {
				case nameof(Source.SortingCondition):
					this.SortingCondition = new SortingCondition(PropertyNameConverter(Source.SortingCondition.PropertyName), Source.SortingCondition.Direction);
					break;
			}
		}
#if(NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER)
		[return: NotNullIfNotNull(nameof(propertyName))]
#endif
		private string? PropertyNameConverter(string? propertyName)
		{
			if(propertyName != null && PropertyNameDictionary.TryGetValue(propertyName, out var ret))
				return ret;
			else
				return propertyName;
		}

#if(NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER)
		[return: NotNullIfNotNull(nameof(propertyName))]
#endif
		private string? PropertyNameBackConverter(string? propertyName)
		{
			if(propertyName != null && PropertyNameBackDictionary.TryGetValue(propertyName, out var ret))
				return ret;
			else
				return propertyName;
		}

		#endregion

		#region SortingCondition変更通知プロパティ

		/// <summary>
		/// 現在のソート条件
		/// </summary>
		public SortingCondition SortingCondition
		{
			get { return _SortingCondition; }
			private set
			{
				if(object.Equals(_SortingCondition, value))
					return;
				_SortingCondition = value;
				RaisePropertyChanged(nameof(SortingCondition));
			}
		}
		private SortingCondition _SortingCondition;

		#endregion

		#region Commands

		/// <summary>
		/// 選択中の項目を削除するコマンド
		/// </summary>
		public ICommand RemoveSelectedItemCommand { get; }

		/// <summary>
		/// 選択中の項目を上へ移動するコマンド
		/// </summary>
		public ICommand MoveUpSelectedItemCommand { get; }

		/// <summary>
		/// 選択中の項目を上へ移動するコマンド
		/// </summary>
		public ICommand MoveDownSelectedItemCommand { get; }

		/// <summary>
		/// プロパティ名をパラメーターに与えてソートするコマンド
		/// </summary>
		public ICommand SortByPropertyCommand { get; }

		/// <summary>
		/// 選択を反転するコマンド
		/// </summary>
		public ICommand ToggleSelectionCommand { get; }

		#endregion

		#region Selection

		#region SelectedItemsSetter変更通知プロパティ

		/// <summary>
		/// ListView.SelectedItemsにバインディングして現在の選択中のアイテムを知らせます
		/// </summary>
		public System.Collections.IList? SelectedItemsSetter
		{
			//get { return _SelectedItems; }
			set
			{
				if(_SelectedItemsSetter == value)
					return;

				if(_SelectedItemsSetter is INotifyCollectionChanged notifyCollectionChanged)
					notifyCollectionChanged.CollectionChanged -= SelectedItemsSetter_CollectionChanged;
				_SelectedItems.Clear();

				_SelectedItemsSetter = value;

				if(_SelectedItemsSetter != null) {
					foreach(var selected in _SelectedItemsSetter.Cast<TViewModel>())
						_SelectedItems.Add(selected);
					if(_SelectedItemsSetter is INotifyCollectionChanged)
						((INotifyCollectionChanged)_SelectedItemsSetter).CollectionChanged += SelectedItemsSetter_CollectionChanged;
				}

				RaisePropertyChanged(nameof(SelectedItemsSetter));
				RaiseRemoveSelectedItemCanExecuteChanged();
				RaiseMoveSelectedItemCanExecuteChanged();
			}
		}
		private System.Collections.IList? _SelectedItemsSetter;

		#endregion

		private void SelectedItemsSetter_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add:
					_SelectedItems.InsertRange(e.NewStartingIndex, e.NewItems?.Cast<TViewModel>() ?? []);
					break;
				case NotifyCollectionChangedAction.Move:
					_SelectedItems.Move(e.OldStartingIndex, e.NewStartingIndex);
					break;
				case NotifyCollectionChangedAction.Remove:
					if(e.OldItems != null) {
						foreach(var r in e.OldItems)
							_SelectedItems.RemoveAt(e.OldStartingIndex);
					}
					break;
				case NotifyCollectionChangedAction.Replace:
					foreach(var r in e.OldItems?.Cast<TViewModel>()?.Zip(e.NewItems?.Cast<TViewModel>() ?? [], (o, n) => new { Old = o, New = n }) ?? [])
						_SelectedItems[_SelectedItems.IndexOf(r.Old)] = r.New;
					break;
				case NotifyCollectionChangedAction.Reset:
					_SelectedItems.Clear();
					_SelectedItems.AddRange(e.NewItems?.Cast<TViewModel>() ?? Enumerable.Empty<TViewModel>());
					break;
			}

			RaiseRemoveSelectedItemCanExecuteChanged();
			RaiseMoveSelectedItemCanExecuteChanged();
		}

		readonly ObservableCollection<TViewModel> _SelectedItems = new ObservableCollection<TViewModel>();

		/// <summary>
		/// 選択中の項目をIListじゃ使いにくいから使いやすくミラーリングしたクラス
		/// </summary>
		public ReadOnlyObservableCollection<TViewModel> SelectedItems
		{
			get
			{
				if(_ReadOnlySelectedItems == null)
					_ReadOnlySelectedItems = new ReadOnlyObservableCollection<TViewModel>(_SelectedItems);
				return _ReadOnlySelectedItems;
			}
		}
		private ReadOnlyObservableCollection<TViewModel>? _ReadOnlySelectedItems;

		#region Methods that change selection

		/// <summary>
		/// 指定したアイテムを選択します
		/// </summary>
		/// <param name="item">選択するアイテム</param>
		public void SelectItem(TViewModel item)
		{
			if(item == null)
				throw new ArgumentNullException(nameof(item));
			if(!this.Contains(item))
				throw new ArgumentException("This collection doesn't contain the item.", nameof(item));
			if(_SelectedItems.Contains(item))
				throw new InvalidOperationException("Already selected.");
			if(_SelectedItemsSetter == null)
				throw new InvalidOperationException($"{nameof(_SelectedItemsSetter)} is not set yet.");

			if(Dispatcher.CheckAccess())
				_SelectedItemsSetter.Add(item);
			else
				Dispatcher.Invoke(DispatcherPriority, (Action)(() => _SelectedItemsSetter.Add(item)));
		}

		/// <summary>
		/// 指定したインデックスの要素を選択します。
		/// </summary>
		/// <param name="index">選択するインデックス</param>
		public void SelectAt(int index) => SelectItem(this[index]);

		/// <summary>
		/// 指定した要素の選択を解除します
		/// </summary>
		/// <param name="item">選択を解除する要素</param>
		public void UnselectItem(TViewModel item)
		{
			if(item == null)
				throw new ArgumentNullException(nameof(item));
			if(!this.Contains(item))
				throw new ArgumentException("This collection doesn't contain the item,");
			if(!_SelectedItems.Contains(item))
				throw new InvalidOperationException("Already selected.");
			if(_SelectedItemsSetter == null)
				throw new InvalidOperationException($"{nameof(_SelectedItemsSetter)} is not set yet.");

			if(Dispatcher.CheckAccess())
				_SelectedItemsSetter.Remove(item);
			else
				Dispatcher.Invoke(DispatcherPriority, () => _SelectedItemsSetter.Remove(item));
		}

		/// <summary>
		/// 指定したインデックスの要素の選択を解除します
		/// </summary>
		/// <param name="index">インデックス</param>
		public void UnselectAt(int index) => UnselectItem(this[index]);

		/// <summary>
		/// 選択されているアイテムかを調べます
		/// </summary>
		/// <param name="item">選択されているかどうか調べたい要素</param>
		/// <returns>選択されていればtrue</returns>
		public bool IsSelectedItem(TViewModel item)
		{
			if(item == null)
				throw new ArgumentNullException(nameof(item));
			if(!this.Contains(item))
				throw new ArgumentException("This collection doesn't contain the item,");
			return _SelectedItems.Contains(item);
		}

		/// <summary>
		/// 選択されているアイテムかを調べます
		/// </summary>
		/// <param name="item">選択されているかどうか調べたい要素</param>
		/// <returns>選択されていればtrue</returns>
		public bool IsSelectedAt(int index) => IsSelectedItem(this[index]);

		/// <summary>
		/// 指定したアイテムの選択を反転します
		/// </summary>
		/// <param name="item">選択を反転するアイテム</param>
		public void ToggleItemSelection(TViewModel item)
		{
			if(IsSelectedItem(item))
				UnselectItem(item);
			else
				SelectItem(item);
		}

		/// <summary>
		/// 指定したインデックスの要素の選択を反転します。
		/// </summary>
		/// <param name="index">インデックス</param>
		public void ToggleItemSelectionAt(int index) => ToggleItemSelection(this[index]);

		#endregion

		protected void RaiseToggleSelectionCanExecuteChanged()
		{
			((Command)ToggleSelectionCommand).RaiseCanExecuteChanged();
		}

		protected void RaiseRemoveSelectedItemCanExecuteChanged()
		{
			((Command)RemoveSelectedItemCommand).RaiseCanExecuteChanged();
		}

		protected void RaiseMoveSelectedItemCanExecuteChanged()
		{
			((Command)MoveUpSelectedItemCommand).RaiseCanExecuteChanged();
			((Command)MoveDownSelectedItemCommand).RaiseCanExecuteChanged();
		}

		#endregion

		#region Lock

		#region UseNonUIThreadWhenCallingModel変更通知プロパティ

		/// <summary>
		/// Modelを呼び出すときにUIスレッド以外のスレッドを使うかどうか
		/// </summary>
		public bool UseNonUIThreadWhenCallingModel
		{
			get => _UseNonUIThreadWhenCallingModel;
			set
			{
				if(_UseNonUIThreadWhenCallingModel == value)
					return;
				_UseNonUIThreadWhenCallingModel = value;
				RaisePropertyChanged(nameof(UseNonUIThreadWhenCallingModel));
			}
		}
		private bool _UseNonUIThreadWhenCallingModel;

		#endregion
		
		#region DisableCommandWhenCommandTaskRunning変更通知プロパティ

		/// <summary>
		/// CommandのTaskが実行中はCommandを利用不可にするかどうか
		/// </summary>
		public bool DisableCommandWhenCommandTaskRunning
		{
			get=> _DisableCommandWhenCommandTaskRunning;
			set
			{
				if(_DisableCommandWhenCommandTaskRunning == value)
					return;
				_DisableCommandWhenCommandTaskRunning = value;
				RaisePropertyChanged(nameof(DisableCommandWhenCommandTaskRunning));
			}
		}
		private bool _DisableCommandWhenCommandTaskRunning;

		#endregion
		
		#region CommandTaskRunning変更通知プロパティ

		/// <summary>
		/// コマンドのタスクが実行中
		/// </summary>
		public bool CommandTaskRunning
		{
			get=> _CommandTaskRunning;
			protected set
			{
				if(_CommandTaskRunning == value)
					return;
				_CommandTaskRunning = value;
				RaisePropertyChanged(nameof(CommandTaskRunning));
			}
		}
		private bool _CommandTaskRunning;

		#endregion
		
		void SafetySourceAccessIfAvailable(Action action)
		{
			if(UseNonUIThreadWhenCallingModel)
				Task.Run(() => ExecActionWithLockIfAvailable(action));
			else
				ExecActionWithLockIfAvailable(action);
		}

		void ExecActionWithLockIfAvailable(Action action)
		{
			if(Source is System.Collections.ICollection collection && collection.IsSynchronized)
				lock(collection.SyncRoot)
					action();
			else
				action();
		}

		#endregion

		#region IDisposable

		protected override void Dispose(bool disposing)
		{
			if(disposing) {
				Source.PropertyChanged -= Source_PropertyChanged;
				this.CollectionChanged -= ListViewViewModel_CollectionChanged;
			}

			base.Dispose(disposing);
		}

		#endregion
	}
}

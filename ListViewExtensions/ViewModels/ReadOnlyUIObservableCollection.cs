using System;

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ListViewExtensions.ViewModels
{
	public class ReadOnlyUIObservableCollection<TViewModel, TModel> : IReadOnlyList<TViewModel>, IReadOnlyCollection<TViewModel>, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable
	{
		readonly IEnumerable<TModel> Source;
		readonly INotifyCollectionChanged SourceAsNotifyCollectionChanged;
		readonly UIObservableCollection<TViewModel> Target;
		readonly Func<TModel, TViewModel> Converter;

		public ReadOnlyUIObservableCollection(IEnumerable<TModel> source, Func<TModel, TViewModel> converter, Dispatcher dispatcher) : this(source, converter, dispatcher, DispatcherPriority.Normal) { }

		public ReadOnlyUIObservableCollection(IEnumerable<TModel> source, Func<TModel, TViewModel> converter, Dispatcher dispatcher, DispatcherPriority priority)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
			Converter = converter ?? throw new ArgumentException(nameof(converter));
			SourceAsNotifyCollectionChanged = Source as INotifyCollectionChanged ?? throw new ArgumentException($"{nameof(source)} must implement INotifyCollectionChanged interface.");

			Target = new UIObservableCollection<TViewModel>(Source.Select(p => Converter(p)), dispatcher);

			SourceAsNotifyCollectionChanged.CollectionChanged += Source_CollectionChanged;
			Target.CollectionChanged += Target_CollectionChanged;
			Target.PropertyChanged += Target_PropertyChanged;
		}

		#region Source/Target Watcher

		private void Target_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RaiseCollectionChanged(e);
		}

		private void Target_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName) {
				case nameof(Target.Dispatcher):
					RaisePropertyChanged(nameof(this.Dispatcher));
					break;
				case nameof(Target.DispatcherPriority):
					RaisePropertyChanged(nameof(this.DispatcherPriority));
					break;
			}
		}

		private void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch(e.Action) {
				case NotifyCollectionChangedAction.Add:
					Target.InsertRange(e.NewStartingIndex, e.NewItems.Cast<TModel>().Select(p => Converter(p)));
					break;
				case NotifyCollectionChangedAction.Move:
					Target.MoveRange(e.OldStartingIndex, e.NewStartingIndex, e.OldItems.Count);
					break;
				case NotifyCollectionChangedAction.Remove:
					TViewModel[] removed = new TViewModel[e.OldItems.Count];
					for(int i = 0; i < e.OldItems.Count; i++)
						removed[i] = Target[e.OldStartingIndex + i];
										
					Target.RemoveRange(e.OldStartingIndex, e.OldItems.Count);

					foreach(var rd in removed.OfType<IDisposable>().Where(p => p != null))
						rd.Dispose();

					break;
				case NotifyCollectionChangedAction.Replace:
					for(int i = 0; i < e.NewItems.Count; i++) {
						var r = Target[e.NewStartingIndex + i];
						Target[e.NewStartingIndex + i] = Converter((TModel)e.NewItems[i]);
						if(r is IDisposable rd)
							rd.Dispose();
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					var buf = Target.ToArray();

					Target.Clear();
					Target.AddRange(e.NewItems?.Cast<TModel>()?.Select(p => Converter(p)) ?? Enumerable.Empty<TViewModel>());

					foreach(var item in buf.OfType<IDisposable>().Where(p => p != null))
						item.Dispose();
					break;
				default:
					throw new ArgumentException(nameof(e.Action));
			}

			if(Target.Count != Source.Count())
				throw new InvalidOperationException($"Source and Target synchronization fault: {e.Action}");
		}

		#endregion

		#region Properties

		/// <summary>
		/// このコレクションが変更通知を行うDispatcher
		/// </summary>
		public Dispatcher Dispatcher
		{
			get
			{
				ThrowExceptionIfDisposed();
				return Target.Dispatcher;
			}
			set
			{
				ThrowExceptionIfDisposed();
				Target.Dispatcher = value;
			}
		}

		/// <summary>
		/// コレクション変更通知時のDispatcherPriority
		/// </summary>
		public DispatcherPriority DispatcherPriority
		{
			get
			{
				ThrowExceptionIfDisposed();
				return Target.DispatcherPriority;
			}
			set
			{
				ThrowExceptionIfDisposed();
				Target.DispatcherPriority = value;
			}
		}

		#endregion

		#region IReadOnlyList<TViewModel> implementation

		/// <summary>
		/// アイテムの個数
		/// </summary>
		public int Count
		{
			get
			{
				ThrowExceptionIfDisposed();
				return Target.Count();
			}
		}

		/// <summary>
		/// 指定したインデックスのアイテムを取得するインデクサ
		/// </summary>
		/// <param name="index">インデックス</param>
		/// <returns>アイテム</returns>
		public TViewModel this[int index]
		{
			get
			{
				ThrowExceptionIfDisposed();
				return Target[index];
			}
		}

		/// <summary>
		/// 指定したアイテムが含まれているかを調べるメソッド
		/// </summary>
		/// <param name="item">アイテム</param>
		/// <returns>含まれていればtrue</returns>
		public bool Contains(TViewModel item)
		{
			ThrowExceptionIfDisposed();
			return Target.Contains(item);
		}

		/// <summary>
		/// 指定したアイテムのインデックスを調べるメソッド
		/// </summary>
		/// <param name="item">アイテム</param>
		/// <returns>インデックス</returns>
		public int IndexOf(TViewModel item)
		{
			ThrowExceptionIfDisposed();
			return Target.IndexOf(item);
		}

		/// <summary>
		/// 指定したアイテムのインデックスを調べるメソッド
		/// </summary>
		/// <param name="item">アイテム</param>
		/// <param name="index">検索をスタートするインデックス</param>
		/// <returns>インデックス</returns>
		public int IndexOf(TViewModel item, int index)
		{
			ThrowExceptionIfDisposed();
			return Target.IndexOf(item, index);
		}

		/// <summary>
		/// 指定したアイテムのインデックスを調べるメソッド
		/// </summary>
		/// <param name="item">アイテム</param>
		/// <param name="index">検索をスタートするインデックス</param>
		/// <param name="count">検索する個数</param>
		/// <returns>インデックス</returns>
		public int IndexOf(TViewModel item, int index, int count)
		{
			ThrowExceptionIfDisposed();
			return Target.IndexOf(item, index, count);
		}

		/// <summary>
		/// 自分自身を指定した配列にコピーするメソッド
		/// </summary>
		/// <param name="array">コピー先</param>
		/// <param name="arrayIndex">コピー先の何番目から書き込むか</param>
		public void CopyTo(TViewModel[] array, int arrayIndex)
		{
			ThrowExceptionIfDisposed();
			Target.CopyTo(array, arrayIndex);
		}

		public IEnumerator<TViewModel> GetEnumerator()
		{
			ThrowExceptionIfDisposed();
			return Target.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		#region INotifyCollectionChanged implementation

		protected virtual void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			if(CollectionChanged != null) {
				if(Dispatcher?.CheckAccess() ?? true)
					CollectionChanged(this, args);
				else
					Dispatcher.Invoke(DispatcherPriority, (Action)(() => CollectionChanged(this, args)));
			}
		}

		/// <summary>
		/// プロパティが変更された際に発生するイベントです。
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		#endregion

		#region INotifyPropertyChanged implementation

		protected virtual void RaisePropertyChanged(string propertyName)
		{
			if(PropertyChanged != null) {
				if(Dispatcher?.CheckAccess() ?? true)
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				else
					Dispatcher.Invoke(DispatcherPriority, (Action)(() => PropertyChanged(this, new PropertyChangedEventArgs(propertyName))));
			}
		}

		/// <summary>
		/// コレクションが変更された際に発生するイベントです。
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region IDispose implementation

		bool _disposed = false;

		/// <summary>
		/// ソースコレクションとの連動を解除します。
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(_disposed)
				return;

			if(disposing) {
				SourceAsNotifyCollectionChanged.CollectionChanged -= Source_CollectionChanged;
				Target.CollectionChanged -= Target_CollectionChanged;
				Target.PropertyChanged -= Target_PropertyChanged;

				foreach(var item in Target.Where(p => p is IDisposable).Select(p => (IDisposable)p))
					item.Dispose();
				Target.Clear();
			}
			_disposed = true;
		}

		~ReadOnlyUIObservableCollection()
		{
			Dispose(false);
		}

		protected void ThrowExceptionIfDisposed()
		{
			if(_disposed)
				throw new ObjectDisposedException(nameof(ReadOnlyUIObservableCollection<TViewModel, TModel>));
		}

		#endregion
	}
}

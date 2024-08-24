using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using ListViewExtensions.Models;

namespace ListViewExtensions.ViewModels
{
	public class UIObservableCollection<T> : SyncedObservableCollection<T>
	{
		#region Constructor

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="dispatcher">UIのDispatcher</param>
		public UIObservableCollection(Dispatcher dispatcher) : this(Enumerable.Empty<T>(), dispatcher) { }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="collection">元となるコレクション</param>
		/// <param name="dispatcher">UIのDispatcher</param>
		public UIObservableCollection(IEnumerable<T> collection, Dispatcher dispatcher) : this(collection, dispatcher, DispatcherPriority.Normal) { }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="collection">元となるコレクション</param>
		/// <param name="dispatcher">UIのDispatcher</param>
		/// <param name="priority">Dispatcherの優先度</param>
		public UIObservableCollection(IEnumerable<T> collection, Dispatcher dispatcher, DispatcherPriority priority) : base(collection)
		{
			Dispatcher = dispatcher;
			DispatcherPriority = priority;
		}

		#endregion

		#region Properties

		/// <summary>
		/// このコレクションが変更通知を行うDispatcherを取得します。
		/// </summary>
		public Dispatcher Dispatcher
		{
			get => Sync.ReadWithLock(() => _Dispatcher);
			set => Sync.UpgradeableReadWithLock(() => {
				if(_Dispatcher == value)
					return;

				Sync.WriteWithLock(() => _Dispatcher = value);

				RaisePropertyChanged(nameof(Dispatcher));
			});
			
		}
		Dispatcher _Dispatcher;

		/// <summary>
		/// コレクション変更通知時のDispatcherPriority
		/// </summary>
		public DispatcherPriority DispatcherPriority
		{
			get => Sync.ReadWithLock(() => _DispatcherPriority);
			set => Sync.UpgradeableReadWithLock(() => {
				if(_DispatcherPriority == value)
					return;

				Sync.WriteWithLock(() => _DispatcherPriority = value);

				RaisePropertyChanged(nameof(DispatcherPriority));
			});
		}
		DispatcherPriority _DispatcherPriority;

		#endregion

		#region IList<T>, ICollection Implement

		protected override void InsertItem(int index, T[] array)
		{
			if(array.Any(p => this.Contains(p)))
				throw new ArgumentException($"Existing item(s) cannot be added.");

			base.InsertItem(index, array);
		}

		#endregion

		#region INotifyCollectionChanged

		protected override void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if(Dispatcher?.CheckAccess() ?? true)
				base.RaiseCollectionChanged(e);
			else
				Dispatcher.Invoke(DispatcherPriority, (Action)(() => base.RaiseCollectionChanged(e)));
		}
		
		#endregion

		#region INotifyPropertyChanged

		protected override void RaisePropertyChanged(PropertyChangedEventArgs e)
		{
			if(Dispatcher?.CheckAccess() ?? true)
				base.RaisePropertyChanged(e);
			else
				Dispatcher.Invoke(DispatcherPriority, (Action)(() => base.RaisePropertyChanged(e)));
		}

		#endregion
	}
}

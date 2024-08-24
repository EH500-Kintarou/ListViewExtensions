using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ListViewExtensions.Util;

namespace ListViewExtensions.Models
{
	public class SortableObservableCollection<T> : SyncedObservableCollection<T>, ISortableObservableCollection<T>
	{
		public SortableObservableCollection(IEnumerable<T> collection) : base(collection)
		{
			_SortingCondition = SortingCondition.None;

			foreach(var item in this)
				StartListeningIfNotifyPropertyChanged(item);
		}
		
		public SortableObservableCollection() : this(Enumerable.Empty<T>()) { }

		#region Sort

		#region SortingCondition変更通知プロパティ

		public SortingCondition SortingCondition
		{
			get => Sync.ReadWithLock(() => _SortingCondition);
			set => Sync.UpgradeableReadWithLock(() => {
				if(_SortingCondition == value)
					return;
				Sync.WriteWithLock(() => _SortingCondition = value);
				RaisePropertyChanged(nameof(SortingCondition));
			});
		}
		private SortingCondition _SortingCondition;

		#endregion

		/// <summary>
		/// 自身をソートします。
		/// </summary>
		/// <param name="propertyName">ソートするプロパティ名</param>
		/// <param name="direction">ソートする方向</param>
		public void Sort(string propertyName, SortingDirection direction)
		{
			if(direction == SortingDirection.None)
				throw new ArgumentException($"\"{nameof(direction)}\" must not be None.");

			var property = typeof(T).GetProperty(propertyName) ?? throw new ArgumentException($"\"{propertyName}\" doesn't exist.");

			Sync.UpgradeableReadWithLock(() => {
				var Sorted = this.OrderByDirection(p => property.GetValue(p), direction, Comparer<object?>.Create((x, y) => CompareProperty(x, y, propertyName))).ToArray();
				Sync.WriteWithLock(() => {
					for(int i = 0; i < Sorted.Length; i++) {
						int oldindex = this.IndexOf(Sorted[i], i);

						if(oldindex < i)
							throw new InvalidOperationException("ソートしたらデータが消えてる…だと…？");
						else if(oldindex > i)
							this.Move(oldindex, i);
					}
					SortingCondition = new SortingCondition(propertyName, direction);
				});
			});
		}

		/// <summary>
		/// プロパティの大小関係を調べます。
		/// ソートをカスタマイズするにはこのメソッドをオーバーライドしてください。
		/// </summary>
		/// <param name="x">1つ目のプロパティの値</param>
		/// <param name="y">2つ目のプロパティの値</param>
		/// <param name="propertyName">プロパティ名</param>
		/// <returns>x > yなら正、x == yなら0、x < yなら負の値</returns>
		public virtual int CompareProperty(object? x, object? y, string propertyName)
		{
			if(x is IComparable xc && y is IComparable yc)
				return xc.CompareTo(yc);
			else
				return 0;
		}

		/// <summary>
		/// コレクションの順番が変わったかもしれないときにこのメソッドを呼び出します。
		/// 例えば、コレクションのプロパティが変化したときなどが該当します。
		/// </summary>
		protected void CollectionOrderMayBeChanged()
		{
			Sync.UpgradeableReadWithLock(() => {
				if(!IsSorted(SortingCondition))
					SortingCondition = SortingCondition.None;
			});
		}

		/// <summary>
		/// 自身が指定されたソート条件に合致しているか調べます。
		/// </summary>
		/// <param name="condition">ソート条件</param>
		/// <returns>合致していればtrue、していなければfalse</returns>
		private bool IsSorted(SortingCondition condition)
		{
			if(condition.IsNone || this.Count <= 1)
				return true;    //条件が無かったり個数が1個以下だったらソートされているといえる

			var property = typeof(T).GetProperty(condition.PropertyName) ?? throw new ArgumentException($"{nameof(condition)}.{nameof(condition.PropertyName)} is not found.", nameof(condition));
			var properties = Sync.ReadWithLock(() => this.Select(p => property.GetValue(p)).ToArray());
			var compare = properties.Zip(properties.Skip(1), (x, y) => CompareProperty(x, y, condition.PropertyName));

			Func<int, bool> predicate = p => true;

			switch(condition.Direction) {
				case SortingDirection.Ascending:
					predicate = p => p <= 0;
					break;
				case SortingDirection.Descending:
					predicate = p => p >= 0;
					break;
			}

			return compare.All(predicate);
		}

		/// <summary>
		/// 自身のソート状態からして適切な位置にアイテムを追加します。
		/// ソートされていなかったら末尾に追加されます。
		/// もしも追加するアイテムと同じ大きさのアイテムがあった場合、すでにあるものの最後に挿入されます。
		/// </summary>
		/// <param name="item">追加するアイテム</param>
		public void AddAsSorted(T item)
		{
			Sync.UpgradeableReadWithLock(() => {
				if(SortingCondition.IsNone) //ソートされていなかったら
					Add(item);
				else {
					var property = typeof(T).GetProperty(SortingCondition.PropertyName) ?? throw new ArgumentException($"{nameof(SortingCondition)}.{nameof(SortingCondition.PropertyName)} is not found.");
					var index = this
						.Concat([item])
						.OrderByDirection(p => property.GetValue(p), SortingCondition.Direction, Comparer<object?>.Create((x, y) => CompareProperty(x, y, SortingCondition.PropertyName)))
						.ToList()
						.IndexOf(item);

					Insert(index, item);
				}
			});
		}

		#endregion

		#region Item Listener

		protected override void SetItem(int index, T item)
		{
			T oldItem = this[index];

			StartListeningIfNotifyPropertyChanged(item);
			base.SetItem(index, item);
			StopListeningIfNotifyPropertyChanged(oldItem);
		}

		protected override void InsertItem(int index, T[] array)
		{
			foreach(var item in array)
				StartListeningIfNotifyPropertyChanged(item);
			base.InsertItem(index, array);
		}

		protected override void RemoveItem(int index, int count)
		{
			T[] oldItems = new T[count];
			for(int i = 0; i < count; i++)
				oldItems[i] = this[index + i];
			
			base.RemoveItem(index, count);

			foreach(var item in oldItems)
				StopListeningIfNotifyPropertyChanged(item);
		}
		
		protected override void ClearItems()
		{
			var AllItems = this.ToArray();

			base.ClearItems();

			foreach(var item in AllItems)
				StopListeningIfNotifyPropertyChanged(item);
		}

		protected override void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if(e.Action != NotifyCollectionChangedAction.Remove)
				CollectionOrderMayBeChanged();	//コレクションの順序が変わったかもしれない

			base.RaiseCollectionChanged(e);
		}

		void StartListeningIfNotifyPropertyChanged(T item)
		{
			if(item is INotifyPropertyChanged propertychanged)
				propertychanged.PropertyChanged += Element_PropertyChanged;
		}

		void StopListeningIfNotifyPropertyChanged(T item)
		{
			if(item is INotifyPropertyChanged propertychanged)
				propertychanged.PropertyChanged -= Element_PropertyChanged;
		}

		private void Element_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == SortingCondition.PropertyName)
				CollectionOrderMayBeChanged();
		}

		#endregion
	}
}

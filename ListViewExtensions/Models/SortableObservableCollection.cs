using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ListViewExtensions.Util;

namespace ListViewExtensions.Models
{
	public class SortableObservableCollection<T> : SyncedObservableCollection<T>, ISortableObservableCollection<T>
	{
		readonly IDictionary<string, System.Collections.IComparer> propertyComparerDictionary;

		public SortableObservableCollection() : this([]) { }

		public SortableObservableCollection(IEnumerable<T> collection) : this(collection, new Dictionary<string, System.Collections.IComparer>()) { }

		public SortableObservableCollection(IDictionary<string, System.Collections.IComparer> propertyComparer) : this([], propertyComparer) { }

		public SortableObservableCollection(IEnumerable<T> collection, IDictionary<string, System.Collections.IComparer> propertyComparer) : base(collection)
		{
			_SortingCondition = SortingCondition.None;
			this.propertyComparerDictionary = propertyComparer;

			foreach(var item in this)
				StartListeningIfNotifyPropertyChanged(item);
		}
		
		#region Sort

		#region SortingCondition変更通知プロパティ

		public SortingCondition SortingCondition
		{
			get => Sync.ReadWithLock(() => _SortingCondition);
			private set => Sync.UpgradeableReadWithLock(() => {
				if(_SortingCondition.Equals(value))
					return;
				Sync.WriteWithLock(() => _SortingCondition = value);
				RaisePropertyChanged(nameof(SortingCondition));
			});
		}
		private SortingCondition _SortingCondition;

		#endregion

		#region Sorting Core

		/// <summary>
		/// 自身をソートするメソッド
		/// </summary>
		/// <param name="direction">ソート方向。Noneの場合は何もしない。</param>
		/// <param name="propertyName">ソートに使用するプロパティ名。Nullの場合は要素自身をキーとしてソート。</param>
		public void Sort(SortingDirection direction, string? propertyName = null)
		{
			Sort(direction, GetPropertyComparer(propertyName));
			SortingCondition = propertyName == null ? new SortingCondition(direction) : new SortingCondition(propertyName, direction);
		}

		/// <summary>
		/// 自身をソートするメソッド
		/// </summary>
		/// <param name="direction">ソート方向。Noneの場合は何もしない。</param>
		/// <param name="comparer">要素同士の比較を提供するComparer</param>
		protected void Sort(SortingDirection direction, IComparer<T> comparer)
		{
			Sync.UpgradeableReadWithLock(() => {
				if(this.Count >= 2) {	// 要素が2個以上あるときにソートを行う（そうでないときはすでにソートされていると言える）
					switch(direction) {
						case SortingDirection.Ascending:
							MergeSort(0, this.Count, comparer);
							break;
						case SortingDirection.Descending:
							MergeSort(0, this.Count, new InvertComparer(comparer));
							break;
					}
				}
			});
		}

		/// <summary>
		/// マージソートをするクラス
		/// </summary>
		/// <param name="left">左端のインデックス（自身を含む）</param>
		/// <param name="right">右端のインデックス（自身を含まない）</param>
		/// <param name="comparer">比較子</param>
		private void MergeSort(int left, int right, IComparer<T> comparer)
		{
			if(right - left <= 1)
				return;

			var half = (left + right) >> 1;

			MergeSort(left, half, comparer);
			MergeSort(half, right, comparer);

			int i = left;
			int j = half;

			while(i < j && j < right) {
				if(comparer.Compare(this[i], this[j]) > 0) {
					this.Move(j, i);
					j++;
				}
				i++;
			}
		}
		
		private IComparer<T> GetPropertyComparer(string? propertyName)
		{
			System.Collections.IComparer? comparer = null;

			if(propertyName != null)
				propertyComparerDictionary.TryGetValue(propertyName, out comparer);

			return new PropertyComparator(propertyName, comparer);
		}

		private IComparer<T> GetPropertyComparer(string? propertyName, SortingDirection direction)
		{
			switch(direction) {
				case SortingDirection.Ascending:
					return GetPropertyComparer(propertyName);
				case SortingDirection.Descending:
					return new InvertComparer(GetPropertyComparer(propertyName));
				default:
					throw new ArgumentException($"{nameof(direction)} must be Ascending or Descending", nameof(direction));
			}
		}

		private IComparer<T> GetPropertyComparer(SortingCondition condition) => GetPropertyComparer(condition.PropertyName, condition.Direction);

		private class InvertComparer : IComparer<T>
		{
			readonly IComparer<T> original;

			public InvertComparer(IComparer<T> original)
			{
				this.original = original;
			}

#if (NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER)
			public int Compare([AllowNull] T x, [AllowNull] T y)
#else
			public int Compare(T x, T y)
#endif
			{
				return -1 * original.Compare(x, y);
			}
		}

		private class PropertyComparator : IComparer<T>
		{
			readonly PropertyInfo? property;
			readonly System.Collections.IComparer? comparer;

			public PropertyComparator(string? propertyName = null, System.Collections.IComparer? comparer = null)
			{
				this.comparer = comparer;

				if(propertyName == null)
					property = null;
				else
					property = typeof(T).GetProperty(propertyName) ?? throw new ArgumentException($@"Property ""{propertyName}"" doesn't exist.");				
			}

#if(NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER)
			public int Compare([AllowNull] T x, [AllowNull] T y)
#else
			public int Compare(T x, T y)
#endif
			{
				object? px, py;

				if(property != null) {
					px = property.GetValue(x);
					py = property.GetValue(y);
				} else {
					px = x;
					py = y;
				}

				if(comparer == null)
					return (px is IComparable xc && py is IComparable yc) ? xc.CompareTo(yc) : 0;
				else
					return comparer.Compare(px, py);
			}
		}

		#endregion

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

			var comparer = GetPropertyComparer(condition);
			return Sync.ReadWithLock(() => this.Zip(this.Skip(1), (x, y) => comparer.Compare(x, y)).All(p => p <= 0));
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
				if(SortingCondition.IsNone || this.Count == 0)	//ソートされていない、もしくは要素数が0だったらそのまま追加
					Add(item);
				else {
					var comparer = GetPropertyComparer(SortingCondition);

					int left = 0, right = this.Count - 1;
					int target;
					do {
						if(comparer.Compare(item, this[left]) < 0) {
							target = left;
							break;
						}
						if(comparer.Compare(item, this[right]) > 0) {
							target = right + 1;
							break;
						}

						int half = (left + right) / 2;
						if(comparer.Compare(item, this[half]) < 0)
							right = half - 1;
						else
							left = half + 1;

						if(left > right) {
							target = left;
							break;
						}
					} while(true);

					Insert(target, item);
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

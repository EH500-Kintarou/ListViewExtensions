using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListViewExtensions.Models
{
	/// <summary>
	/// ソート可能な変更通知コレクションのインターフェースです。
	/// </summary>
	/// <typeparam name="T">要素の型</typeparam>
    public interface ISortableObservableCollection<T> : IList<T>, INotifyPropertyChanged, INotifyCollectionChanged
	{
		/// <summary>
		/// 自身のソート状態からして適切な位置にアイテムを追加します。
		/// ソートされていなかったら末尾に追加されます。
		/// もしも追加するアイテムと同じ大きさのアイテムがあった場合、すでにあるものの最後に挿入されます。
		/// </summary>
		/// <param name="item">追加するアイテム</param>
		void AddAsSorted(T item);

		/// <summary>
		/// 指定したインデックスが示す位置にある項目を、コレクション内の新しい場所に移動します。
		/// </summary>
		/// <param name="oldIndex">移動する項目の場所を指定する、0から始まるインデックス。</param>
		/// <param name="newIndex">項目の新しい場所を指定する、0から始まるインデックス。</param>
		void Move(int oldIndex, int newIndex);

		/// <summary>
		/// 自身をソートします。
		/// </summary>
		/// <param name="propertyName">ソートするプロパティ名</param>
		/// <param name="direction">ソートする方向</param>
		void Sort(string propertyName, SortingDirection direction);

		/// <summary>
		/// 現在のソート条件
		/// </summary>
		SortingCondition SortingCondition { get; }
	}
}

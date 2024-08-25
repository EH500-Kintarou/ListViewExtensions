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

		[Obsolete]
		void Sort(string propertyName, SortingDirection direction);

		/// <summary>
		/// 自身をソートするメソッド
		/// </summary>
		/// <param name="direction">ソート方向。Noneの場合は何もしない。</param>
		/// <param name="propertyName">ソートに使用するプロパティ名。Nullの場合は要素自身をキーとしてソート。</param>
		void Sort(SortingDirection direction, string? propertyName = null);

		/// <summary>
		/// 現在のソート条件
		/// </summary>
		SortingCondition SortingCondition { get; }
	}
}

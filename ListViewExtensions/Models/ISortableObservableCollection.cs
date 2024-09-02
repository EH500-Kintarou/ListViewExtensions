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
	/// ソート可能な変更通知コレクションのインターフェース
	/// </summary>
	/// <typeparam name="T">要素の型</typeparam>
    public interface ISortableObservableCollection<T> : IList<T>, IReadOnlySortableObservableCollection<T>
	{
		/// <summary>
		/// 自身のソート状態からして適切な位置にアイテムを追加するメソッド
		/// ソートされていなかったら末尾に追加されます。
		/// もしも追加するアイテムと同じ順序のアイテムがあった場合、すでにあるものの最後に挿入されます。
		/// </summary>
		/// <param name="item">追加するアイテム</param>
		void AddAsSorted(T item);
	}
}

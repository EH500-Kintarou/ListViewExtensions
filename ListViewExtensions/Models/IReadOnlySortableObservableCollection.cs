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
	/// ソート可能な読み取り専用の変更通知コレクションのインターフェース
	/// </summary>
	/// <typeparam name="T">要素の型</typeparam>
	public interface IReadOnlySortableObservableCollection<T> : IReadOnlyList<T>, INotifyPropertyChanged, INotifyCollectionChanged
	{
		/// <summary>
		/// 指定したインデックスが示す位置にある項目を、コレクション内の新しい場所に移動するメソッド
		/// </summary>
		/// <param name="oldIndex">移動する項目の場所を指定する、0から始まるインデックス。</param>
		/// <param name="newIndex">項目の新しい場所を指定する、0から始まるインデックス。</param>
		void Move(int oldIndex, int newIndex);

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

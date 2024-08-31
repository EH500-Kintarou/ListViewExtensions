using ListViewExtensions.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace ListViewExtensions.ViewModels
{
	public interface ISelectableCollectionViewModel<T>
	{
		/// <summary>
		/// 選択中の項目を削除するコマンド
		/// </summary>
		ICommand RemoveSelectedItemCommand { get; }

		/// <summary>
		/// 選択中の項目を上へ移動するコマンド
		/// </summary>
		ICommand MoveUpSelectedItemCommand { get; }

		/// <summary>
		/// 選択中の項目を上へ移動するコマンド
		/// </summary>
		ICommand MoveDownSelectedItemCommand { get; }

		/// <summary>
		/// 選択を反転するコマンド
		/// </summary>
		ICommand ToggleSelectionCommand { get; }

		/// <summary>
		/// ListView.SelectedItemsにバインディングして現在の選択中のアイテムを知らせます
		/// </summary>
		System.Collections.IList? SelectedItemsSetter
		{
			set;
		}

		/// <summary>
		/// 選択中の項目をIListじゃ使いにくいから使いやすくミラーリングしたクラス
		/// </summary>
		public ReadOnlyObservableCollection<T> SelectedItems
		{
			get;
		}

		/// <summary>
		/// 指定したアイテムを選択します
		/// </summary>
		/// <param name="item">選択するアイテム</param>
		void SelectItem(T item);

		/// <summary>
		/// 指定したインデックスの要素を選択します。
		/// </summary>
		/// <param name="index">選択するインデックス</param>
		public void SelectAt(int index);

		/// <summary>
		/// 指定した要素の選択を解除します
		/// </summary>
		/// <param name="item">選択を解除する要素</param>
		public void UnselectItem(T item);

		/// <summary>
		/// 指定したインデックスの要素の選択を解除します
		/// </summary>
		/// <param name="index">インデックス</param>
		public void UnselectAt(int index);

		/// <summary>
		/// 選択されているアイテムかを調べます
		/// </summary>
		/// <param name="item">選択されているかどうか調べたい要素</param>
		/// <returns>選択されていればtrue</returns>
		public bool IsSelectedItem(T item);

		/// <summary>
		/// 選択されているアイテムかを調べます
		/// </summary>
		/// <param name="item">選択されているかどうか調べたい要素</param>
		/// <returns>選択されていればtrue</returns>
		public bool IsSelectedAt(int index);

		/// <summary>
		/// 指定したアイテムの選択を反転します
		/// </summary>
		/// <param name="item">選択を反転するアイテム</param>
		public void ToggleItemSelection(T item);

		/// <summary>
		/// 指定したインデックスの要素の選択を反転します。
		/// </summary>
		/// <param name="index">インデックス</param>
		public void ToggleItemSelectionAt(int index);
	}
}

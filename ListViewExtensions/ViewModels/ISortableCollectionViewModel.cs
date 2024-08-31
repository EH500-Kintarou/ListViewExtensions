using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ListViewExtensions.ViewModels
{
    public interface ISortableCollectionViewModel
	{
		/// <summary>
		/// 現在のソート条件
		/// </summary>
		SortingCondition SortingCondition
		{
			get;
		}

		/// <summary>
		/// プロパティ名をパラメーターに与えてソートするコマンド
		/// </summary>
		ICommand SortByPropertyCommand { get; }
	}
}

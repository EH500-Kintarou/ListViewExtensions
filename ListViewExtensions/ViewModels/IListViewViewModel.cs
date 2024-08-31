using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListViewExtensions.ViewModels
{
	public interface IListViewViewModel<T> : ISortableCollectionViewModel, ISelectableCollectionViewModel<T>
	{
	}
}

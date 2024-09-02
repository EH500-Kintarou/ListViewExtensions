using ListViewExtensions.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest.Models.ObservableCollectionTestBase;

namespace UnitTest.Models.ReadOnlySortableObservableCollection
{
	[TestClass]
	public class ClearTest : ClearTestBase
	{
		protected override (IList<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) TargetFactory<T>(IList<T> items)
		{
			var target = new SortableObservableCollection<T>(items);
			var reader = new ReadOnlySortableObservableCollection<T>(target);

			return (target, reader, reader);
		}

		protected override (IList<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) ModelFactory<T>(IList<T> items)
		{
			var model = new ObservableCollection<T>(items);
			var reader = new ReadOnlyObservableCollection<T>(model);

			return (model, reader, reader);
		}
	}
}

using ListViewExtensions.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest.Models.ObservableCollectionTestBase;

namespace UnitTest.Models.ReadOnlySortableObservableCollection
{
	[TestClass]
	public class RemoveRangeTest : RemoveRangeTestBase
	{
		protected override (SyncedObservableCollection<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) TargetFactory<T>(IList<T> items)
		{
			var target = new SortableObservableCollection<T>(items);
			var reader = new ReadOnlySortableObservableCollection<T>(target);

			return (target, reader, reader);
		}
	}
}

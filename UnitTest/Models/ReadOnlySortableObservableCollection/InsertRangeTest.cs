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
	public class InsertRangeTest : InsertRangeTestBase
	{
		protected override (SyncedObservableCollection<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) TargetFactory<T>()
		{
			var target = new SortableObservableCollection<T>();
			var reader = new ReadOnlySortableObservableCollection<T>(target);
			return (target, reader, reader);
		}

		protected override (List<T> writer, IReadOnlyList<T> reader) ModelFactory<T>()
		{
			var model = new List<T>();
			var reader = new ReadOnlyCollection<T>(model);
			return (model, reader);
		}
	}
}

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

namespace UnitTest.Models.SyncedObservableCollection
{
	[TestClass]
	public class InsertTest : InsertTestBase
	{
		protected override (IList<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) TargetFactory<T>()
		{
			var target = new SyncedObservableCollection<T>();
			return (target, target, target);
		}

		protected override (IList<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) ModelFactory<T>()
		{
			var model = new ObservableCollection<T>();
			return (model, model, model);
		}
	}
}

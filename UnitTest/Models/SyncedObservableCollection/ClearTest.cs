using ListViewExtensions.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Models.SyncedObservableCollection
{
	[TestClass]
	public class ClearTest
	{
		[TestMethod]
		public void ClearTestInt()
		{
			ClearTestInternal([1, 2, 3, 4, 5]);
		}

		[TestMethod]
		public void ClearTestString()
		{
			ClearTestInternal(["hoge", "foo", "bar"]);
		}

		static void ClearTestInternal<T>(IList<T> items)
		{
			var target = new SyncedObservableCollection<T>(items);
			var model = new ObservableCollection<T>(items);

			var tt = new ClearEventTest<T>(items);
			var mt = new ClearEventTest<T>(items);
			target.CollectionChanged += tt.EventListener;
			model.CollectionChanged += mt.EventListener;

			target.Clear();
			model.Clear();

			CollectionAssert.AreEqual(new T[0], target);
			CollectionAssert.AreEqual(new T[0], model);
			Assert.AreEqual(1, tt.Count);
			Assert.AreEqual(1, mt.Count);
		}

		class ClearEventTest<T> : NotifyCollectionChangedEventBase<T>
		{
			public ClearEventTest(IList<T> Itemsed) : base(Itemsed) { }

			public override void EventListener(object? sender, NotifyCollectionChangedEventArgs e)
			{
				Assert.AreEqual(NotifyCollectionChangedAction.Reset, e.Action);
				Assert.AreEqual(-1, e.OldStartingIndex);
				Assert.AreEqual(-1, e.NewStartingIndex);
				Assert.IsNull(e.OldItems);
				Assert.IsNull(e.NewItems);

				count++;
			}
		}
	}
}

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
	public class RemoveTest
	{
		[TestMethod]
		public void RemoveTestInt()
		{
			RemoveTestInternal([1, 2, 3, 4, 5]);
		}

		[TestMethod]
		public void RemoveTestString()
		{
			RemoveTestInternal(["hoge", "foo", "bar"]);
		}

		static void RemoveTestInternal<T>(IList<T> items)
		{
			var target = new SyncedObservableCollection<T>(items);
			var model = new ObservableCollection<T>(items);

			var tt = new RemoveEventTest<T>(items);
			var mt = new RemoveEventTest<T>(items);
			target.CollectionChanged += tt.EventListener;
			model.CollectionChanged += mt.EventListener;

			foreach(var item in items) {
				target.Remove(item);
				model.Remove(item);
			}

			CollectionAssert.AreEqual(new T[0], target);
			CollectionAssert.AreEqual(new T[0], model);
			Assert.AreEqual(items.Count, tt.Count);
			Assert.AreEqual(items.Count, mt.Count);
		}

		class RemoveEventTest<T> : NotifyCollectionChangedEventBase<T>
		{
			public RemoveEventTest(IList<T> Itemsed) : base(Itemsed) { }

			public override void EventListener(object? sender, NotifyCollectionChangedEventArgs e)
			{
				Assert.AreEqual(NotifyCollectionChangedAction.Remove, e.Action);
				Assert.AreEqual(0, e.OldStartingIndex);
				Assert.AreEqual(-1, e.NewStartingIndex);
				Assert.IsNotNull(e.OldItems);
				Assert.IsNull(e.NewItems);
				Assert.AreEqual(1, e.OldItems.Count);
				Assert.AreEqual(items[count], e.OldItems[0]);

				count++;
			}
		}
	}
}

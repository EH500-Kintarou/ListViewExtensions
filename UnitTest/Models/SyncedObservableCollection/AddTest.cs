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
	public class AddTest
	{
		[TestMethod]
		public void AddTestInt()
		{
			AddTestInternal([1, 2, 3, 4, 5]);
		}

		[TestMethod]
		public void AddTestString()
		{
			AddTestInternal(["hoge", "foo", "bar"]);
		}

		static void AddTestInternal<T>(IList<T> items)
		{
			var target = new SyncedObservableCollection<T>();
			var model = new ObservableCollection<T>();

			var tt = new AddEventTest<T>(items);
			var mt = new AddEventTest<T>(items);
			target.CollectionChanged += tt.EventListener;
			model.CollectionChanged += mt.EventListener;

			foreach(var item in items) {
				target.Add(item);
				model.Add(item);
			}

			CollectionAssert.AreEqual(items.ToArray(), target);
			CollectionAssert.AreEqual(items.ToArray(), model);
			Assert.AreEqual(items.Count, tt.Count);
			Assert.AreEqual(items.Count, mt.Count);
		}

		class AddEventTest<T> : NotifyCollectionChangedEventBase<T>
		{
			public AddEventTest(IList<T> Itemsed) : base(Itemsed) { }

			public override void EventListener(object? sender, NotifyCollectionChangedEventArgs e)
			{
				Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
				Assert.AreEqual(-1, e.OldStartingIndex);
				Assert.AreEqual(count, e.NewStartingIndex);
				Assert.IsNull(e.OldItems);
				Assert.IsNotNull(e.NewItems);
				Assert.AreEqual(1, e.NewItems.Count);
				Assert.AreEqual(items[count], e.NewItems[0]);

				count++;
			}
		}
	}
}

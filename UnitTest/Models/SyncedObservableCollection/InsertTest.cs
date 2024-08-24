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
	public class InsertTest
	{
		[TestMethod]
		public void InsertTestInt()
		{
			InsertTestInternal([1, 2, 3, 4, 5, 6]);
		}

		[TestMethod]
		public void InsertTestString()
		{
			InsertTestInternal(["hoge", "foo", "bar", "hogehoge", "foofoo", "barbar"]);
		}

		static void InsertTestInternal<T>(IList<T> items)
		{
			var target = new SyncedObservableCollection<T>();
			var model = new ObservableCollection<T>();

			var tt = new InsertEventTest<T>(items);
			var mt = new InsertEventTest<T>(items);
			target.CollectionChanged += tt.EventListener;
			model.CollectionChanged += mt.EventListener;

			for(int i = 0; i < items.Count; i++) {
				var item = items[i];

				switch(i % 3) {
					case 0:
						target.Insert(0, item);
						model.Insert(0, item);
						break;
					case 1:
						target.Insert(target.Count / 2, item);
						model.Insert(model.Count / 2, item);
						break;
					case 2:
						target.Insert(target.Count, item);
						model.Insert(model.Count, item);
						break;
				}
			}

			CollectionAssert.AreEqual(model, target);
			Assert.AreEqual(items.Count, tt.Count);
			Assert.AreEqual(items.Count, mt.Count);
		}

		class InsertEventTest<T> : NotifyCollectionChangedEventBase<T>
		{
			public InsertEventTest(IList<T> Itemsed) : base(Itemsed) { }

			public override void EventListener(object? sender, NotifyCollectionChangedEventArgs e)
			{
				Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
				Assert.AreEqual(-1, e.OldStartingIndex);
				Assert.AreEqual((count % 3) switch { 0 => 0, 1 => count / 2, 2 => count, _ => throw new AssertFailedException("Invalid remainder") }, e.NewStartingIndex);
				Assert.IsNull(e.OldItems);
				Assert.IsNotNull(e.NewItems);
				Assert.AreEqual(1, e.NewItems.Count);
				Assert.AreEqual(items[count], e.NewItems[0]);

				count++;
			}
		}
	}
}

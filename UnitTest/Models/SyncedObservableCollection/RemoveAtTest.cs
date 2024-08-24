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
	public class RemoveAtTest
	{
		[TestMethod]
		public void RemoveAtTestInt()
		{
			RemoveAtTestInternal([1, 2, 3, 4, 5, 6]);
		}

		[TestMethod]
		public void RemoveAtTestString()
		{
			RemoveAtTestInternal(["hoge", "foo", "bar", "hogehoge", "foofoo", "barbar"]);
		}

		static void RemoveAtTestInternal<T>(IList<T> items)
		{
			var target = new SyncedObservableCollection<T>(items);
			var model = new ObservableCollection<T>(items);

			var tt = new RemoveAtEventTest<T>(items);
			var mt = new RemoveAtEventTest<T>(items);
			target.CollectionChanged += tt.EventListener;
			model.CollectionChanged += mt.EventListener;

			for(int i = 0; i < items.Count; i++) {
				switch(i % 3) {
					case 0:
						target.RemoveAt(0);
						model.RemoveAt(0);
						break;
					case 1:
						target.RemoveAt(target.Count / 2);
						model.RemoveAt(model.Count / 2);
						break;
					case 2:
						target.RemoveAt(target.Count - 1);
						model.RemoveAt(model.Count - 1);
						break;
				}
			}

			CollectionAssert.AreEqual(new T[0], target);
			CollectionAssert.AreEqual(new T[0], model);
			Assert.AreEqual(items.Count, tt.Count);
			Assert.AreEqual(items.Count, mt.Count);
		}

		class RemoveAtEventTest<T> : NotifyCollectionChangedEventBase<T>
		{
			List<T> remaining;

			public RemoveAtEventTest(IList<T> Items) : base(Items)
			{
				remaining = new List<T>(Items);
			}

			public override void EventListener(object? sender, NotifyCollectionChangedEventArgs e)
			{
				Assert.AreEqual(NotifyCollectionChangedAction.Remove, e.Action);
				Assert.AreEqual((count % 3) switch { 0 => 0, 1 => (items.Count - count) / 2, 2 => items.Count - count - 1, _ => throw new AssertFailedException("Invalid remainder") }, e.OldStartingIndex);
				Assert.AreEqual(-1, e.NewStartingIndex);
				Assert.IsNotNull(e.OldItems);
				Assert.IsNull(e.NewItems);
				Assert.AreEqual(1, e.OldItems.Count);

				var removed = (T?)e.OldItems[0];
				
				Assert.IsNotNull(removed);
				Assert.IsTrue(remaining.Contains(removed));
				remaining.Remove(removed);

				count++;
			}
		}
	}
}

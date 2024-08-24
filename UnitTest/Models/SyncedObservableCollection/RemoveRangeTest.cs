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
	public class RemoveRangeTest
	{
		[TestMethod]
		public void RemoveRangeTestInt()
		{
			RemoveRangeTestInternal([1, 2, 3, 4, 5, 6]);
		}

		[TestMethod]
		public void RemoveRangeTestString()
		{
			RemoveRangeTestInternal(["hoge", "foo", "bar", "hogehoge", "foofoo", "barbar"]);
		}

		static void RemoveRangeTestInternal<T>(IList<T> items)
		{
			var target = new SyncedObservableCollection<T>(items);

			var tt = new RemoveRangeEventTest<T>(items);
			target.CollectionChanged += tt.EventListener;

			target.RemoveRange(2, 3);

			CollectionAssert.AreEqual(items.Take(2).Concat(items.Skip(5)).ToArray(), target);
			Assert.AreEqual(1, tt.Count);
		}

		class RemoveRangeEventTest<T> : NotifyCollectionChangedEventBase<T>
		{
			public RemoveRangeEventTest(IList<T> Itemsed) : base(Itemsed) { }

			public override void EventListener(object? sender, NotifyCollectionChangedEventArgs e)
			{
				Assert.AreEqual(NotifyCollectionChangedAction.Remove, e.Action);
				Assert.AreEqual(2, e.OldStartingIndex);
				Assert.AreEqual(-1, e.NewStartingIndex);
				Assert.IsNotNull(e.OldItems);
				Assert.IsNull(e.NewItems);
				Assert.AreEqual(3, e.OldItems.Count);
				CollectionAssert.AreEqual(items.Skip(2).Take(3).ToArray(), e.OldItems);

				count++;
			}
		}
	}
}

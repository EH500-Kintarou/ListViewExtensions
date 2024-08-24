using ListViewExtensions.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Models.SyncedObservableCollection
{
	[TestClass]
	public class AddRangeTest
	{
		[TestMethod]
		public void AddRangeTestInt()
		{
			AddRangeTestInternal([1, 2, 3, 4, 5]);
		}

		[TestMethod]
		public void AddRangeTestString()
		{
			AddRangeTestInternal(["hoge", "foo", "bar"]);
		}

		static void AddRangeTestInternal<T>(IList<T> items)
		{
			var target = new SyncedObservableCollection<T>();

			var tt = new AddRangeEventTest<T>(items);
			target.CollectionChanged += tt.EventListener;

			target.AddRange(items);

			CollectionAssert.AreEqual(items.ToArray(), target);
			Assert.AreEqual(1, tt.Count);
		}

		class AddRangeEventTest<T> : NotifyCollectionChangedEventBase<T>
		{
			public AddRangeEventTest(IList<T> Itemsed) : base(Itemsed) { }

			public override void EventListener(object? sender, NotifyCollectionChangedEventArgs e)
			{
				Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
				Assert.AreEqual(-1, e.OldStartingIndex);
				Assert.AreEqual(count, e.NewStartingIndex);
				Assert.IsNull(e.OldItems);
				Assert.IsNotNull(e.NewItems);
				CollectionAssert.AreEqual(items.ToArray(), e.NewItems);

				count++;
			}
		}
	}
}

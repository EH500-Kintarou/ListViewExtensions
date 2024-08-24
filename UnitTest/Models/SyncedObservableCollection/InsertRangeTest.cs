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
	public class InsertRangeTest
	{
		#region InsertRangeTest

		[TestMethod]
		public void InsertRangeTestInt()
		{
			InsertRangeTestInternal([1, 2, 3, 4, 5, 6]);
		}

		[TestMethod]
		public void InsertRangeTestString()
		{
			InsertRangeTestInternal(["hoge", "foo", "bar", "hogehoge", "foofoo", "barbar"]);
		}

		static void InsertRangeTestInternal<T>(IList<T> items)
		{
			var target = new SyncedObservableCollection<T>();
			var model = new List<T>();

			var tt = new InsertRangeEventTest<T>(items);
			target.CollectionChanged += tt.EventListener;

			for(int i = 0; i < items.Count / 2; i++) {
				var item = items.Skip(i * 2).Take(2);

				switch(i % 3) {
					case 0:
						target.InsertRange(0, item);
						model.InsertRange(0, item);
						break;
					case 1:
						target.InsertRange(target.Count / 2, item);
						model.InsertRange(model.Count / 2, item);
						break;
					case 2:
						target.InsertRange(target.Count, item);
						model.InsertRange(model.Count, item);
						break;
				}
			}

			CollectionAssert.AreEqual(model, target);
			Assert.AreEqual(items.Count / 2, tt.Count);
		}

		class InsertRangeEventTest<T> : NotifyCollectionChangedEventBase<T>
		{
			public InsertRangeEventTest(IList<T> Items) : base(Items) { }

			public override void EventListener(object? sender, NotifyCollectionChangedEventArgs e)
			{
				Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
				Assert.AreEqual(-1, e.OldStartingIndex);
				Assert.AreEqual((count % 3) switch { 0 => 0, 1 => count, 2 => count * 2, _ => throw new AssertFailedException("Invalid remainder") }, e.NewStartingIndex);
				Assert.IsNull(e.OldItems);
				Assert.IsNotNull(e.NewItems);
				CollectionAssert.AreEqual(items.Skip(count * 2).Take(2).ToArray(), e.NewItems);

				count++;
			}
		}

		#endregion
	}
}

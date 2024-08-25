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
    public class ReplaceTest
    {
		[TestMethod]
		public void ReplaceTestInt()
		{
			ReplaceTestInternal([1, 2, 3, 4, 5], 0);
		}

		[TestMethod]
		public void ReplaceTestString()
		{
			ReplaceTestInternal(["hoge", "foo", "bar"], "replace");
		}

		static void ReplaceTestInternal<T>(IList<T> items, T replace)
		{
			var target = new SyncedObservableCollection<T>(items);
			var model = new ObservableCollection<T>(items);

			var tt = new ReplaceEventTest<T>(items);
			var mt = new ReplaceEventTest<T>(items);
			target.CollectionChanged += tt.EventListener;
			model.CollectionChanged += mt.EventListener;

			int[] indeces = [0, items.Count / 2, items.Count - 1];

			for(int i = 0; i < indeces.Length; i++) {
				var index = indeces[i];

				tt.ExpectedIndex = mt.ExpectedIndex = index;
				tt.ExpectedOld = mt.ExpectedOld = items[index];
				tt.ExpectedNew = mt.ExpectedNew = replace;
				target[index] = replace;
				model[index] = replace;

				CollectionAssert.AreEqual(model, target);
				Assert.AreEqual(i + 1, tt.Count);
				Assert.AreEqual(i + 1, mt.Count);
			}
		}

		class ReplaceEventTest<T> : NotifyCollectionChangedEventBase<T>
		{
			public ReplaceEventTest(IList<T> Itemsed) : base(Itemsed) { }

			public override void EventListener(object? sender, NotifyCollectionChangedEventArgs e)
			{
				Assert.AreEqual(NotifyCollectionChangedAction.Replace, e.Action);
				Assert.AreEqual(ExpectedIndex, e.OldStartingIndex);
				Assert.AreEqual(ExpectedIndex, e.NewStartingIndex);
				Assert.IsNotNull(e.OldItems);
				Assert.IsNotNull(e.NewItems);
				Assert.AreEqual(1, e.OldItems.Count);
				Assert.AreEqual(1, e.NewItems.Count);
				Assert.AreEqual(ExpectedOld, e.OldItems[0]);
				Assert.AreEqual(ExpectedNew, e.NewItems[0]);

				count++;
			}

			public int ExpectedIndex { get; set; }
			public T? ExpectedOld { get; set; }
			public T? ExpectedNew { get; set; }
		}
	}
}

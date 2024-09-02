using ListViewExtensions.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest.Models.SyncedObservableCollection;

namespace UnitTest.Models.ObservableCollectionTestBase
{
	public abstract class RemoveRangeTestBase
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

		protected abstract (SyncedObservableCollection<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) TargetFactory<T>(IList<T> items);

		void RemoveRangeTestInternal<T>(IList<T> items)
		{
			(var twriter, var treader, var twatcher) = TargetFactory<T>(items);

			var tt = new RemoveRangeEventTest<T>(items);
			twatcher.CollectionChanged += tt.EventListener;

			twriter.RemoveRange(2, 3);

			CollectionAssert.AreEqual(items.Take(2).Concat(items.Skip(5)).ToArray(), treader.ToArray());
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest.Models.SyncedObservableCollection;

namespace UnitTest.Models.ObservableCollectionTestBase
{
	public abstract class RemoveAtTestBase
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

		protected abstract (IList<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) TargetFactory<T>(IList<T> items);
		protected abstract (IList<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) ModelFactory<T>(IList<T> items);

		void RemoveAtTestInternal<T>(IList<T> items)
		{
			(var twriter, var treader, var twatcher) = TargetFactory<T>(items);
			(var mwriter, var mreader, var mwatcher) = ModelFactory<T>(items);

			var tt = new RemoveAtEventTest<T>(items);
			var mt = new RemoveAtEventTest<T>(items);
			twatcher.CollectionChanged += tt.EventListener;
			mwatcher.CollectionChanged += mt.EventListener;

			for(int i = 0; i < items.Count; i++) {
				switch(i % 3) {
					case 0:
						twriter.RemoveAt(0);
						mwriter.RemoveAt(0);
						break;
					case 1:
						twriter.RemoveAt(twriter.Count / 2);
						mwriter.RemoveAt(mwriter.Count / 2);
						break;
					case 2:
						twriter.RemoveAt(twriter.Count - 1);
						mwriter.RemoveAt(mwriter.Count - 1);
						break;
				}
			}

			CollectionAssert.AreEqual(new T[0], treader.ToArray());
			CollectionAssert.AreEqual(new T[0], mreader.ToArray());
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

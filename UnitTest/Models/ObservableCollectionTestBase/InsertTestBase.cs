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
	public abstract class InsertTestBase
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

		protected abstract (IList<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) TargetFactory<T>();
		protected abstract (IList<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) ModelFactory<T>();

		void InsertTestInternal<T>(IList<T> items)
		{
			(var twriter, var treader, var twatcher) = TargetFactory<T>();
			(var mwriter, var mreader, var mwatcher) = ModelFactory<T>();

			var tt = new InsertEventTest<T>(items);
			var mt = new InsertEventTest<T>(items);
			twatcher.CollectionChanged += tt.EventListener;
			mwatcher.CollectionChanged += mt.EventListener;

			for(int i = 0; i < items.Count; i++) {
				var item = items[i];

				switch(i % 3) {
					case 0:
						twriter.Insert(0, item);
						mwriter.Insert(0, item);
						break;
					case 1:
						twriter.Insert(twriter.Count / 2, item);
						mwriter.Insert(mwriter.Count / 2, item);
						break;
					case 2:
						twriter.Insert(twriter.Count, item);
						mwriter.Insert(mwriter.Count, item);
						break;
				}
			}

			CollectionAssert.AreEqual(mreader.ToArray(), treader.ToArray());
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

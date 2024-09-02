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
	public abstract class RemoveTestBase
	{
		[TestMethod]
		public void RemoveTestInt()
		{
			RemoveTestInternal([1, 2, 3, 4, 5]);
		}

		[TestMethod]
		public void RemoveTestString()
		{
			RemoveTestInternal(["hoge", "foo", "bar"]);
		}

		protected abstract (IList<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) TargetFactory<T>(IList<T> items);
		protected abstract (IList<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) ModelFactory<T>(IList<T> items);
		
		void RemoveTestInternal<T>(IList<T> items)
		{
			(var twriter, var treader, var twatcher) = TargetFactory<T>(items);
			(var mwriter, var mreader, var mwatcher) = ModelFactory<T>(items);

			var tt = new RemoveEventTest<T>(items);
			var mt = new RemoveEventTest<T>(items);
			twatcher.CollectionChanged += tt.EventListener;
			mwatcher.CollectionChanged += mt.EventListener;

			foreach(var item in items) {
				twriter.Remove(item);
				mwriter.Remove(item);
			}

			CollectionAssert.AreEqual(new T[0], treader.ToArray());
			CollectionAssert.AreEqual(new T[0], mreader.ToArray());
			Assert.AreEqual(items.Count, tt.Count);
			Assert.AreEqual(items.Count, mt.Count);
		}

		class RemoveEventTest<T> : NotifyCollectionChangedEventBase<T>
		{
			public RemoveEventTest(IList<T> Itemsed) : base(Itemsed) { }

			public override void EventListener(object? sender, NotifyCollectionChangedEventArgs e)
			{
				Assert.AreEqual(NotifyCollectionChangedAction.Remove, e.Action);
				Assert.AreEqual(0, e.OldStartingIndex);
				Assert.AreEqual(-1, e.NewStartingIndex);
				Assert.IsNotNull(e.OldItems);
				Assert.IsNull(e.NewItems);
				Assert.AreEqual(1, e.OldItems.Count);
				Assert.AreEqual(items[count], e.OldItems[0]);

				count++;
			}
		}
	}
}

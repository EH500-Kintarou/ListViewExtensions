using ListViewExtensions.Models;
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
	public abstract class AddTestBase
	{
		[TestMethod]
		public void AddTestInt()
		{
			AddTestInternal([1, 2, 3, 4, 5]);
		}

		[TestMethod]
		public void AddTestString()
		{
			AddTestInternal(["hoge", "foo", "bar"]);
		}

		protected abstract (IList<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) TargetFactory<T>();
		protected abstract (IList<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) ModelFactory<T>();


		void AddTestInternal<T>(IList<T> items)
		{
			(var twriter, var treader, var twatcher) = TargetFactory<T>();
			(var mwriter, var mreader, var mwatcher) = ModelFactory<T>();

			var tt = new AddEventTest<T>(items);
			var mt = new AddEventTest<T>(items);
			twatcher.CollectionChanged += tt.EventListener;
			mwatcher.CollectionChanged += mt.EventListener;

			foreach(var item in items) {
				twriter.Add(item);
				mwriter.Add(item);
			}

			CollectionAssert.AreEqual(items.ToArray(), treader.ToArray());
			CollectionAssert.AreEqual(items.ToArray(), mreader.ToArray());
			Assert.AreEqual(items.Count, tt.Count);
			Assert.AreEqual(items.Count, mt.Count);
		}

		class AddEventTest<T> : NotifyCollectionChangedEventBase<T>
		{
			public AddEventTest(IList<T> Itemsed) : base(Itemsed) { }

			public override void EventListener(object? sender, NotifyCollectionChangedEventArgs e)
			{
				Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
				Assert.AreEqual(-1, e.OldStartingIndex);
				Assert.AreEqual(count, e.NewStartingIndex);
				Assert.IsNull(e.OldItems);
				Assert.IsNotNull(e.NewItems);
				Assert.AreEqual(1, e.NewItems.Count);
				Assert.AreEqual(items[count], e.NewItems[0]);

				count++;
			}
		}
	}
}

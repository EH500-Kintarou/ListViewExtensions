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
	public abstract class ClearTestBase
	{
		[TestMethod]
		public void ClearTestInt()
		{
			ClearTestInternal([1, 2, 3, 4, 5]);
		}

		[TestMethod]
		public void ClearTestString()
		{
			ClearTestInternal(["hoge", "foo", "bar"]);
		}

		protected abstract (IList<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) TargetFactory<T>(IList<T> items);
		protected abstract (IList<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) ModelFactory<T>(IList<T> items);


		void ClearTestInternal<T>(IList<T> items)
		{
			(var twriter, var treader, var twatcher) = TargetFactory<T>(items);
			(var mwriter, var mreader, var mwatcher) = ModelFactory<T>(items);

			var tt = new ClearEventTest<T>(items);
			var mt = new ClearEventTest<T>(items);
			twatcher.CollectionChanged += tt.EventListener;
			mwatcher.CollectionChanged += mt.EventListener;

			twriter.Clear();
			mwriter.Clear();

			CollectionAssert.AreEqual(new T[0], treader.ToArray());
			CollectionAssert.AreEqual(new T[0], mreader.ToArray());
			Assert.AreEqual(1, tt.Count);
			Assert.AreEqual(1, mt.Count);
		}

		class ClearEventTest<T> : NotifyCollectionChangedEventBase<T>
		{
			public ClearEventTest(IList<T> Itemsed) : base(Itemsed) { }

			public override void EventListener(object? sender, NotifyCollectionChangedEventArgs e)
			{
				Assert.AreEqual(NotifyCollectionChangedAction.Reset, e.Action);
				Assert.AreEqual(-1, e.OldStartingIndex);
				Assert.AreEqual(-1, e.NewStartingIndex);
				Assert.IsNull(e.OldItems);
				Assert.IsNull(e.NewItems);

				count++;
			}
		}
	}
}

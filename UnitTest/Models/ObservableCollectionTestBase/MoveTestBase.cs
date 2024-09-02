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
	public abstract class MoveTestBase
	{
		[TestMethod]
		public void MoveTestInt()
		{
			MoveTestInternal([1, 2, 3, 4, 5]);
		}

		[TestMethod]
		public void MoveTestString()
		{
			MoveTestInternal(["hoge", "foo", "bar"]);
		}

		protected abstract (SyncedObservableCollection<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) TargetFactory<T>(IList<T> items);
		protected abstract (ObservableCollection<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) ModelFactory<T>(IList<T> items);

		void MoveTestInternal<T>(IList<T> items)
		{
			(var twriter, var treader, var twatcher) = TargetFactory<T>(items);
			(var mwriter, var mreader, var mwatcher) = ModelFactory<T>(items);

			var tt = new MoveEventTest<T>(items);
			var mt = new MoveEventTest<T>(items);
			twatcher.CollectionChanged += tt.EventListener;
			mwatcher.CollectionChanged += mt.EventListener;

			(int oldIndex, int newIndex)[] replace = [(0, items.Count - 1), (items.Count - 1, 0), (items.Count / 2, items.Count / 2)];

			for(int i = 0; i < replace.Length; i++) {
				(var oldIndex, var newIndex) = replace[i];

				tt.ExpectedOldIndex = mt.ExpectedOldIndex = oldIndex;
				tt.ExpectedNewIndex = mt.ExpectedNewIndex = newIndex;
				tt.ExpectedItem = mt.ExpectedItem = mreader[oldIndex];

				twriter.Move(oldIndex, newIndex);
				mwriter.Move(oldIndex, newIndex);

				CollectionAssert.AreEqual(mreader.ToArray(), treader.ToArray());
				Assert.AreEqual(i + 1, tt.Count);
				Assert.AreEqual(i + 1, mt.Count);
			}
		}

		class MoveEventTest<T> : NotifyCollectionChangedEventBase<T>
		{
			public MoveEventTest(IList<T> Itemsed) : base(Itemsed) { }

			public override void EventListener(object? sender, NotifyCollectionChangedEventArgs e)
			{
				Assert.AreEqual(NotifyCollectionChangedAction.Move, e.Action);
				Assert.AreEqual(ExpectedOldIndex, e.OldStartingIndex);
				Assert.AreEqual(ExpectedNewIndex, e.NewStartingIndex);
				Assert.IsNotNull(e.OldItems);
				Assert.IsNotNull(e.NewItems);
				Assert.AreEqual(1, e.OldItems.Count);
				Assert.AreEqual(1, e.NewItems.Count);
				Assert.AreEqual(ExpectedItem, e.OldItems[0]);
				Assert.AreEqual(ExpectedItem, e.NewItems[0]);

				count++;
			}

			public int ExpectedOldIndex { get; set; }
			public int ExpectedNewIndex { get; set; }

			public T? ExpectedItem { get; set; }
		}
	}
}

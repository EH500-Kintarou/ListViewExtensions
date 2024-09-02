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
	public abstract class MoveRangeTestBase
	{
		[TestMethod]
		public void MoveRangeTestInt()
		{
			MoveRangeTestInternal([1, 2, 3, 4, 5]);
		}

		[TestMethod]
		public void MoveRangeTestString()
		{
			MoveRangeTestInternal(["hoge", "foo", "bar"]);
		}

		protected abstract (SyncedObservableCollection<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) TargetFactory<T>(IList<T> items);
		protected abstract (ObservableCollection<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) ModelFactory<T>(IList<T> items);

		void MoveRangeTestInternal<T>(IList<T> items)
		{
			(var twriter, var treader, var twatcher) = TargetFactory<T>(items);
			(var mwriter, var mreader, var mwatcher) = ModelFactory<T>(items);

			var tt = new MoveRangeEventTest<T>(items);
			var mt = new MoveRangeEventTest<T>(items);
			twatcher.CollectionChanged += tt.EventListener;
			mwatcher.CollectionChanged += mt.EventListener;

			(int oldIndex, int newIndex)[] replace = [(0, items.Count - 2), (items.Count - 2, 0), (items.Count / 2, items.Count / 2)];

			for(int i = 0; i < replace.Length; i++) {
				(var oldIndex, var newIndex) = replace[i];

				tt.ExpectedOldIndex = oldIndex;
				tt.ExpectedNewIndex = newIndex;
				tt.ExpectedItems = treader.Skip(oldIndex).Take(2).ToArray();

				twriter.MoveRange(oldIndex, newIndex, 2);

				if(oldIndex < newIndex) {
					mt.ExpectedOldIndex = oldIndex;
					mt.ExpectedNewIndex = newIndex + 1;
					mt.ExpectedItems = [mreader[oldIndex]];
					mwriter.Move(oldIndex, newIndex + 1);

					mt.ExpectedItems = [mreader[oldIndex]];
					mwriter.Move(oldIndex, newIndex + 1);
				} else {
					mt.ExpectedOldIndex = oldIndex;
					mt.ExpectedNewIndex = newIndex;
					mt.ExpectedItems = [mreader[oldIndex]];
					mwriter.Move(oldIndex, newIndex);

					mt.ExpectedOldIndex = oldIndex + 1;
					mt.ExpectedNewIndex = newIndex + 1;
					mt.ExpectedItems = [mreader[oldIndex + 1]];
					mwriter.Move(oldIndex + 1, newIndex + 1);
				}

				CollectionAssert.AreEqual(mreader.ToArray(), treader.ToArray());
				Assert.AreEqual(i + 1, tt.Count);
				Assert.AreEqual((i + 1) * 2, mt.Count);
			}
		}

		class MoveRangeEventTest<T> : NotifyCollectionChangedEventBase<T>
		{
			public MoveRangeEventTest(IList<T> Itemsed) : base(Itemsed) { }

			public override void EventListener(object? sender, NotifyCollectionChangedEventArgs e)
			{
				Assert.AreEqual(NotifyCollectionChangedAction.Move, e.Action);
				Assert.AreEqual(ExpectedOldIndex, e.OldStartingIndex);
				Assert.AreEqual(ExpectedNewIndex, e.NewStartingIndex);
				Assert.IsNotNull(e.OldItems);
				Assert.IsNotNull(e.NewItems);
				Assert.IsNotNull(ExpectedItems);
				Assert.AreEqual(ExpectedItems.Length, e.OldItems.Count);
				Assert.AreEqual(ExpectedItems.Length, e.NewItems.Count);
				CollectionAssert.AreEqual(ExpectedItems, e.OldItems);
				CollectionAssert.AreEqual(ExpectedItems, e.NewItems);

				count++;
			}

			public int ExpectedOldIndex { get; set; }
			public int ExpectedNewIndex { get; set; }

			public T[]? ExpectedItems { get; set; }
		}
	}
}

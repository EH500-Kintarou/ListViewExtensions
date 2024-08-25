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
	public class MoveRangeTest
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

		static void MoveRangeTestInternal<T>(IList<T> items)
		{
			var target = new SyncedObservableCollection<T>(items);
			var model = new ObservableCollection<T>(items);

			var tt = new MoveRangeEventTest<T>(items);
			var mt = new MoveRangeEventTest<T>(items);
			target.CollectionChanged += tt.EventListener;
			model.CollectionChanged += mt.EventListener;

			(int oldIndex, int newIndex)[] replace = [(0, items.Count - 2), (items.Count - 2, 0), (items.Count / 2, items.Count / 2)];

			for(int i = 0; i < replace.Length; i++) {
				(var oldIndex, var newIndex) = replace[i];

				tt.ExpectedOldIndex = oldIndex;
				tt.ExpectedNewIndex = newIndex;
				tt.ExpectedItems = target.Skip(oldIndex).Take(2).ToArray();

				target.MoveRange(oldIndex, newIndex, 2);

				if(oldIndex < newIndex) {
					mt.ExpectedOldIndex = oldIndex;
					mt.ExpectedNewIndex = newIndex + 1;
					mt.ExpectedItems = [model[oldIndex]];
					model.Move(oldIndex, newIndex + 1);

					mt.ExpectedItems = [model[oldIndex]];
					model.Move(oldIndex, newIndex + 1);
				} else {
					mt.ExpectedOldIndex = oldIndex;
					mt.ExpectedNewIndex = newIndex;
					mt.ExpectedItems = [model[oldIndex]];
					model.Move(oldIndex, newIndex);

					mt.ExpectedOldIndex = oldIndex + 1;
					mt.ExpectedNewIndex = newIndex + 1;
					mt.ExpectedItems = [model[oldIndex + 1]];
					model.Move(oldIndex + 1, newIndex + 1);
				}

				CollectionAssert.AreEqual(model, target);
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

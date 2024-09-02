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
	public abstract class AddRangeTestBase
	{
		[TestMethod]
		public void AddRangeTestInt()
		{
			AddRangeTestInternal([1, 2, 3, 4, 5]);
		}

		[TestMethod]
		public void AddRangeTestString()
		{
			AddRangeTestInternal(["hoge", "foo", "bar"]);
		}

		protected abstract (SyncedObservableCollection<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) TargetFactory<T>();

		void AddRangeTestInternal<T>(IList<T> items)
		{
			(var target, var reader, var watcher) = TargetFactory<T>();

			var tt = new AddRangeEventTest<T>(items);
			watcher.CollectionChanged += tt.EventListener;

			target.AddRange(items);

			CollectionAssert.AreEqual(items.ToArray(), reader.ToArray());
			Assert.AreEqual(1, tt.Count);
		}

		class AddRangeEventTest<T> : NotifyCollectionChangedEventBase<T>
		{
			public AddRangeEventTest(IList<T> Itemsed) : base(Itemsed) { }

			public override void EventListener(object? sender, NotifyCollectionChangedEventArgs e)
			{
				Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
				Assert.AreEqual(-1, e.OldStartingIndex);
				Assert.AreEqual(count, e.NewStartingIndex);
				Assert.IsNull(e.OldItems);
				Assert.IsNotNull(e.NewItems);
				CollectionAssert.AreEqual(items.ToArray(), e.NewItems);

				count++;
			}
		}

	}
}

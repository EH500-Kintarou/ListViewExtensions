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
	public abstract class InsertRangeTestBase
	{
		[TestMethod]
		public void InsertRangeTestInt()
		{
			InsertRangeTestInternal([1, 2, 3, 4, 5, 6]);
		}

		[TestMethod]
		public void InsertRangeTestString()
		{
			InsertRangeTestInternal(["hoge", "foo", "bar", "hogehoge", "foofoo", "barbar"]);
		}

		protected abstract (SyncedObservableCollection<T> writer, IReadOnlyList<T> reader, INotifyCollectionChanged watcher) TargetFactory<T>();
		protected abstract (List<T> writer, IReadOnlyList<T> reader) ModelFactory<T>();

		void InsertRangeTestInternal<T>(IList<T> items)
		{
			(var twriter, var treader, var twatcher) = TargetFactory<T>();
			(var mwriter, var mreader) = ModelFactory<T>();


			var tt = new InsertRangeEventTest<T>(items);
			twatcher.CollectionChanged += tt.EventListener;

			for(int i = 0; i < items.Count / 2; i++) {
				var item = items.Skip(i * 2).Take(2);

				switch(i % 3) {
					case 0:
						twriter.InsertRange(0, item);
						mwriter.InsertRange(0, item);
						break;
					case 1:
						twriter.InsertRange(twriter.Count / 2, item);
						mwriter.InsertRange(mwriter.Count / 2, item);
						break;
					case 2:
						twriter.InsertRange(twriter.Count, item);
						mwriter.InsertRange(mwriter.Count, item);
						break;
				}
			}

			CollectionAssert.AreEqual(mreader.ToArray(), treader.ToArray());
			Assert.AreEqual(items.Count / 2, tt.Count);
		}

		class InsertRangeEventTest<T> : NotifyCollectionChangedEventBase<T>
		{
			public InsertRangeEventTest(IList<T> Items) : base(Items) { }

			public override void EventListener(object? sender, NotifyCollectionChangedEventArgs e)
			{
				Assert.AreEqual(NotifyCollectionChangedAction.Add, e.Action);
				Assert.AreEqual(-1, e.OldStartingIndex);
				Assert.AreEqual((count % 3) switch { 0 => 0, 1 => count, 2 => count * 2, _ => throw new AssertFailedException("Invalid remainder") }, e.NewStartingIndex);
				Assert.IsNull(e.OldItems);
				Assert.IsNotNull(e.NewItems);
				CollectionAssert.AreEqual(items.Skip(count * 2).Take(2).ToArray(), e.NewItems);

				count++;
			}
		}
	}
}

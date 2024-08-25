using ListViewExtensions;
using ListViewExtensions.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Models.SortableObservableCollection
{
	[TestClass]
	public class AddAsSortedTest
	{
		[TestMethod]
		public void AddAsSortedTestInt()
		{
			AddAsSortedTestInternal([1, 1, 3, 5, 7, 9, 0, 8, 6, 4, 2, 1, 3, 5, 7, 9, 0, 8, 6, 4, 2]);
		}

		[TestMethod]
		public void AddAsSortedTestString()
		{
			AddAsSortedTestInternal(["hoge", "hoge", "foo", "bar", "barbar", "foofoo", "hogehoge", "hoge", "foo", "bar", "barbar", "foofoo", "hogehoge"]);
		}

		[TestMethod]
		public void AddAsSortedTestClass()
		{
			Person[] array = [
				new Person() { Name = "宮内 れんげ", Pronunciation = "みやうち れんげ"  , Birthday = new DateTime(2008, 12,  3), Age =  7, Height_cm = 139, },
				new Person() { Name = "一条 蛍"    , Pronunciation = "いちじょう ほたる", Birthday = new DateTime(2004,  5, 28), Age = 11, Height_cm = 164, },
				new Person() { Name = "越谷 夏海"  , Pronunciation = "こしがや なつみ"  , Birthday = new DateTime(2003,  1, 24), Age = 12, Height_cm = 155, },
				new Person() { Name = "越谷 小毬"  , Pronunciation = "こしがや こまり"  , Birthday = new DateTime(2001,  9, 14), Age = 14, Height_cm = 140, },
				new Person() { Name = "越谷 卓"    , Pronunciation = "こしがや すぐる"  , Birthday = new DateTime(2000,  4, 11), Age = 15, Height_cm = 171, },
				new Person() { Name = "宮内 れんげ", Pronunciation = "みやうち れんげ"  , Birthday = new DateTime(2008, 12,  3), Age =  7, Height_cm = 139, },
				new Person() { Name = "一条 蛍"    , Pronunciation = "いちじょう ほたる", Birthday = new DateTime(2004,  5, 28), Age = 11, Height_cm = 164, },
				new Person() { Name = "越谷 夏海"  , Pronunciation = "こしがや なつみ"  , Birthday = new DateTime(2003,  1, 24), Age = 12, Height_cm = 155, },
				new Person() { Name = "越谷 小毬"  , Pronunciation = "こしがや こまり"  , Birthday = new DateTime(2001,  9, 14), Age = 14, Height_cm = 140, },
				new Person() { Name = "越谷 卓"    , Pronunciation = "こしがや すぐる"  , Birthday = new DateTime(2000,  4, 11), Age = 15, Height_cm = 171, },
			];

			//AddAsSortedTestInternal(array);
			AddAsSortedTestInternal(array, nameof(Person.Name));
			AddAsSortedTestInternal(array, nameof(Person.Pronunciation));
			AddAsSortedTestInternal(array, nameof(Person.Birthday));
			AddAsSortedTestInternal(array, nameof(Person.Age));
			AddAsSortedTestInternal(array, nameof(Person.Height_cm));
		}

		static void AddAsSortedTestInternal<T>(IList<T> items, string? propertyName = null)
		{
			AddAsSortedTestAscending(items, propertyName);
			AddAsSortedTestDescending(items, propertyName);
			AddAsSortedTestNone(items, propertyName);
		}

		static void AddAsSortedTestAscending<T>(IList<T> items, string? propertyName)
		{
			var target = new SortableObservableCollection<T>();
			target.Sort(SortingDirection.Ascending, propertyName);

			for(int i = 0; i < items.Count; i++) {
				var item = items[i];
				target.AddAsSorted(item);

				var sorted = items
					.Take(i + 1)
					.OrderBy(p => propertyName == null ? p : (typeof(T).GetProperty(propertyName) ?? throw new InvalidOperationException("Property value cannot be gotten")).GetValue(p))
					.ToArray();

				CollectionAssert.AreEqual(sorted, target);
			}
		}

		static void AddAsSortedTestDescending<T>(IList<T> items, string? propertyName)
		{
			var target = new SortableObservableCollection<T>();
			target.Sort(SortingDirection.Descending, propertyName);

			for(int i = 0; i < items.Count; i++) {
				var item = items[i];
				target.AddAsSorted(item);

				var sorted = items
					.Take(i + 1)
					.OrderByDescending(p => propertyName == null ? p : (typeof(T).GetProperty(propertyName) ?? throw new InvalidOperationException("Property value cannot be gotten")).GetValue(p))
					.ToArray();

				CollectionAssert.AreEqual(sorted, target);
			}
		}

		static void AddAsSortedTestNone<T>(IList<T> items, string? propertyName)
		{
			var target = new SortableObservableCollection<T>();
			target.Sort(SortingDirection.None, propertyName);

			for(int i = 0; i < items.Count; i++) {
				var item = items[i];
				target.AddAsSorted(item);

				var sorted = items
					.Take(i + 1)
					.ToArray();

				CollectionAssert.AreEqual(sorted, target);
			}
		}
	}
}

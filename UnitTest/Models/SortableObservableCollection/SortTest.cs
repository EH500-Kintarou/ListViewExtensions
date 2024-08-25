using ListViewExtensions;
using ListViewExtensions.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Models.SortableObservableCollection
{
	[TestClass]
	public class SortTest
	{
		[TestMethod]
		public void SortTestInt()
		{
			SortTestInternal([1, 3, 5, 7, 9, 0, 8, 6, 4, 2, 1, 3, 5, 7, 9, 0, 8, 6, 4, 2]);
		}

		[TestMethod]
		public void SortTestString()
		{
			SortTestInternal(["hoge", "foo", "bar", "barbar", "foofoo", "hogehoge", "hoge", "foo", "bar", "barbar", "foofoo", "hogehoge"]);
		}

		[TestMethod]
		public void SortTestClass()
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

			//SortTestInternal(array);
			SortTestInternal(array, nameof(Person.Name));
			SortTestInternal(array, nameof(Person.Pronunciation));
			SortTestInternal(array, nameof(Person.Birthday));
			SortTestInternal(array, nameof(Person.Age));
			SortTestInternal(array, nameof(Person.Height_cm));
		}

		static void SortTestInternal<T>(IList<T> items, string? propertyName = null)
		{
			SortTestAscending(items, propertyName);
			SortTestDescending(items, propertyName);
			SortTestNone(items, propertyName);
		}

		static void SortTestAscending<T>(IList<T> items, string? propertyName)
		{
			var target = new SortableObservableCollection<T>(items);
			var sorted = items
				.OrderBy(p => propertyName == null ? p : (typeof(T).GetProperty(propertyName) ?? throw new InvalidOperationException("Property value cannot be gotten")).GetValue(p))
				.ToArray();

			target.Sort(SortingDirection.Ascending, propertyName);

			CollectionAssert.AreEqual(sorted, target);
		}

		static void SortTestDescending<T>(IList<T> items, string? propertyName)
		{
			var target = new SortableObservableCollection<T>(items);
			var sorted = items
				.OrderByDescending(p => propertyName == null ? p : (typeof(T).GetProperty(propertyName) ?? throw new InvalidOperationException("Property value cannot be gotten")).GetValue(p))
				.ToArray();

			target.Sort(SortingDirection.Descending, propertyName);

			CollectionAssert.AreEqual(sorted, target);
		}

		static void SortTestNone<T>(IList<T> items, string? propertyName)
		{
			var target = new SortableObservableCollection<T>(items);

			target.Sort(SortingDirection.None, propertyName);

			CollectionAssert.AreEqual(items.ToArray(), target);
		}
	}
}

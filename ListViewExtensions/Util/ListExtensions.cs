using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ListViewExtensions.Util
{
	internal static class ListExtensions
	{ 
		internal static void AddRange<T>(this IList<T> source, IEnumerable<T> items)
		{
			if(source == null)
				throw new ArgumentNullException(nameof(source));
			if(items == null)
				throw new ArgumentNullException(nameof(items));

			foreach(T item in items)
				source.Add(item);
		}

		internal static void InsertRange<T>(this IList<T> source, int index, IEnumerable<T> items)
		{
			if(source == null)
				throw new ArgumentNullException(nameof(source));
			if(items == null)
				throw new ArgumentNullException(nameof(items));


			foreach(T item in items)
				source.Insert(index++, item);
		}
		
		internal static IOrderedEnumerable<T> OrderByDirection<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, SortingDirection direction)
		{
			return OrderByDirection(source, keySelector, direction, Comparer<TKey>.Default);
		}

		internal static IOrderedEnumerable<T> OrderByDirection<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, SortingDirection direction, IComparer<TKey> comparer)
		{
			if(source == null)
				throw new ArgumentNullException(nameof(source));
			if(keySelector == null)
				throw new ArgumentNullException(nameof(keySelector));
			if(comparer == null)
				throw new ArgumentNullException(nameof(comparer));

			switch(direction) {
				case SortingDirection.Ascending:
					return source.OrderBy(keySelector, comparer);
				case SortingDirection.Descending:
					return source.OrderByDescending(keySelector, comparer);
				default:
					throw new ArgumentNullException($"{nameof(direction)} must not Ascending or Descending.");
			}
		}
	}
}

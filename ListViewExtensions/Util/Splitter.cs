using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListViewExtensions.Util
{
	internal static class Splitter
	{
		/// <summary>
		/// シーケンスを一定個数ごとに区切るメソッド
		/// </summary>
		/// <typeparam name="TSource">要素の型</typeparam>
		/// <param name="Source">シーケンス</param>
		/// <param name="Count">区切る個数</param>
		/// <returns>区切られたシーケンス</returns>
		public static IEnumerable<IReadOnlyList<TSource>> Split<TSource>(this IEnumerable<TSource> Source, int Count)
		{
			if(Source == null)
				throw new ArgumentNullException(nameof(Source));
			if(Count <= 0)
				throw new ArgumentOutOfRangeException($"\"{nameof(Count)}\" must be bigger than zero.");

			return SplitIterator(Source, Count, p => p);
		}

		/// <summary>
		/// シーケンスを指定した個数ごとに区切るメソッド
		/// </summary>
		/// <typeparam name="TSource">要素の型</typeparam>
		/// <param name="Source">シーケンス</param>
		/// <param name="InitialCount">最初の個数</param>
		/// <param name="NextCountSelector">次の個数を選択するメソッド</param>
		/// <returns>区切られたシーケンス</returns>
		public static IEnumerable<IReadOnlyList<TSource>> Split<TSource>(this IEnumerable<TSource> Source, int InitialCount, Func<int, int> NextCountSelector)
		{
			if(Source == null)
				throw new ArgumentNullException(nameof(Source));
			if(NextCountSelector == null)
				throw new ArgumentNullException(nameof(NextCountSelector));
			if(InitialCount <= 0)
				throw new ArgumentOutOfRangeException($"\"{nameof(InitialCount)}\" must be bigger than zero.");

			return SplitIterator(Source, InitialCount, NextCountSelector);
		}

		private static IEnumerable<IReadOnlyList<TSource>> SplitIterator<TSource>(IEnumerable<TSource> Source, int InitialCount, Func<int, int> NextCountSelector)
		{
			List<TSource> ret = new List<TSource>();
			int counter = 0;
			int NowCount = InitialCount;
			bool CountFinished = false;

			foreach(var s in Source) {
				if(CountFinished) {
					NowCount = NextCountSelector(NowCount);
					if(NowCount <= 0)
						throw new ArgumentOutOfRangeException("The count must be larger than zero.");
					CountFinished = false;
				}
				ret.Add(s);
				if(++counter >= NowCount) {
					yield return ret.AsReadOnly();
					ret.Clear();
					counter = 0;
					CountFinished = true;
				}
			}
			if(ret.Count > 0)
				yield return ret.AsReadOnly();
		}

		/// <summary>
		/// シーケンスを指定された要素の手前で区切るメソッド
		/// </summary>
		/// <typeparam name="TSource">要素の型</typeparam>
		/// <param name="Source">シーケンス</param>
		/// <param name="FirstElementSelector">最初の要素を選択するメソッド</param>
		/// <param name="ReturnEmptyIfFirstestElementIsFirst">もしシーケンスの先頭が区切り位置の直後だった場合、最初に空のリストを返すかどうか。デフォルトでfalse。</param>
		/// <returns>区切られたシーケンス</returns>
		public static IEnumerable<IReadOnlyList<TSource>> SplitByFirst<TSource>(this IEnumerable<TSource> Source, Func<TSource, bool> FirstElementSelector, bool ReturnEmptyIfFirstestElementIsFirst = false)
		{
			if(Source == null)
				throw new ArgumentNullException(nameof(Source));
			if(FirstElementSelector == null)
				throw new ArgumentNullException(nameof(FirstElementSelector));

			return SplitByFirstIterator(Source, FirstElementSelector, ReturnEmptyIfFirstestElementIsFirst);
		}

		private static IEnumerable<IReadOnlyList<TSource>> SplitByFirstIterator<TSource>(IEnumerable<TSource> Source, Func<TSource, bool> FirstElementSelector, bool ReturnEmptyIfFirstestElementIsFirst)
		{
			List<TSource> ret = new List<TSource>();

			foreach(var s in Source) {
				if((ReturnEmptyIfFirstestElementIsFirst || ret.Count > 0) && FirstElementSelector(s)) {
					yield return ret.AsReadOnly();
					ret.Clear();
				}
				ret.Add(s);
			}
			yield return ret.AsReadOnly();
		}

		/// <summary>
		/// シーケンスを指定された要素の直後で区切るメソッド
		/// </summary>
		/// <typeparam name="TSource">要素の型</typeparam>
		/// <param name="Source">シーケンス</param>
		/// <param name="LastElementSelector">最後の要素を選択するメソッド</param>
		/// <param name="ReturnEmptyIfLastestElementIsLast">もしシーケンスの先頭が区切り位置の直後だった場合、最初に空のリストを返すかどうか。デフォルトでfalse。</param>
		/// <returns>区切られたシーケンス</returns>
		public static IEnumerable<IReadOnlyList<TSource>> SplitByLast<TSource>(this IEnumerable<TSource> Source, Func<TSource, bool> LastElementSelector, bool ReturnEmptyIfLastestElementIsLast = false)
		{
			if(Source == null)
				throw new ArgumentNullException(nameof(Source));
			if(LastElementSelector == null)
				throw new ArgumentNullException(nameof(LastElementSelector));

			return SplitByLastIterator(Source, LastElementSelector, ReturnEmptyIfLastestElementIsLast);
		}

		private static IEnumerable<IReadOnlyList<TSource>> SplitByLastIterator<TSource>(IEnumerable<TSource> Source, Func<TSource, bool> LastElementSelector, bool ReturnEmptyIfLastestElementIsLast)
		{
			List<TSource> ret = new List<TSource>();

			foreach(var s in Source) {
				ret.Add(s);
				if(LastElementSelector(s)) {
					yield return ret.AsReadOnly();
					ret.Clear();
				}
			}
			if(ret.Count > 0 || ReturnEmptyIfLastestElementIsLast)
				yield return ret.AsReadOnly();
		}

		/// <summary>
		/// シーケンスを前後の要素で区切るメソッド
		/// </summary>
		/// <typeparam name="TSource">要素の型</typeparam>
		/// <param name="Source">シーケンス</param>
		/// <param name="StridingElementPairSelector">1つ目の引数に前の値、2つ目の引数に後の値を受け取り、切れ目かどうかを判断するメソッド</param>
		/// <returns>区切られたシーケンス</returns>
		public static IEnumerable<IReadOnlyList<TSource>> Split<TSource>(this IEnumerable<TSource> Source, Func<TSource, TSource, bool> StridingElementPairSelector)
		{
			if(Source == null)
				throw new ArgumentNullException(nameof(Source));
			if(StridingElementPairSelector == null)
				throw new ArgumentNullException(nameof(StridingElementPairSelector));

			return SplitIterator(Source, StridingElementPairSelector);
		}

		private static IEnumerable<IReadOnlyList<TSource>> SplitIterator<TSource>(this IEnumerable<TSource> Source, Func<TSource, TSource, bool> StridingElementPairSelector)
		{
			List<TSource> ret = new List<TSource>();

			var enumerator = Source.GetEnumerator();

			if(!enumerator.MoveNext())
				yield break;

			TSource prev = enumerator.Current;
			ret.Add(enumerator.Current);

			while(enumerator.MoveNext()) {
				if(StridingElementPairSelector(prev, enumerator.Current)) {
					yield return ret.AsReadOnly();
					ret.Clear();
				}
				prev = enumerator.Current;
				ret.Add(enumerator.Current);
			}
			yield return ret.AsReadOnly();
		}
	}
}

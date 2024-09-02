using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.Models.ObservableCollectionTestBase
{
	internal abstract class NotifyCollectionChangedEventBase<T>
	{
		protected int count = 0;
		protected readonly IList<T> items;

		public NotifyCollectionChangedEventBase(IList<T> Items)
		{
			items = Items;
		}

		public abstract void EventListener(object? sender, NotifyCollectionChangedEventArgs e);

		public int Count { get => count; }
	}
}

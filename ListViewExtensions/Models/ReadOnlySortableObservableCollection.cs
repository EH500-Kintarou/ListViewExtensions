using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListViewExtensions.Models
{
	public class ReadOnlySortableObservableCollection<T> : IReadOnlySortableObservableCollection<T>, IDisposable
	{
		const string IndexerName = "Item[]";

		IReadOnlySortableObservableCollection<T>? source;

		#region Constructor

		public ReadOnlySortableObservableCollection(IReadOnlySortableObservableCollection<T> source)
		{
			this.source = source;

			source.PropertyChanged += Source_PropertyChanged;
			source.CollectionChanged += Source_CollectionChanged;
		}

		#endregion

		#region IReadOnlyList<T>

		public T this[int index]
		{
			get
			{
				if(source == null) throw new ObjectDisposedException(nameof(source));
				return source[index];
			}
		}

		public int Count
		{
			get
			{
				if(source == null) throw new ObjectDisposedException(nameof(source));
				return source.Count;
			}
		}

		#endregion

		#region IReadOnlySortableObservableCollection<T>

		/// <summary>
		/// 指定したインデックスが示す位置にある項目を、コレクション内の新しい場所に移動するメソッド
		/// </summary>
		/// <param name="oldIndex">移動する項目の場所を指定する、0から始まるインデックス。</param>
		/// <param name="newIndex">項目の新しい場所を指定する、0から始まるインデックス。</param>
		public void Move(int oldIndex, int newIndex)
		{
			if(source == null) throw new ObjectDisposedException(nameof(source));
			source.Move(oldIndex, newIndex);
		}

		/// <summary>
		/// 自身をソートするメソッド
		/// </summary>
		/// <param name="direction">ソート方向。Noneの場合は何もしない。</param>
		/// <param name="propertyName">ソートに使用するプロパティ名。Nullの場合は要素自身をキーとしてソート。</param>
		public void Sort(SortingDirection direction, string? propertyName = null)
		{
			if(source == null) throw new ObjectDisposedException(nameof(source));
			source.Sort(direction, propertyName);
		}

		/// <summary>
		/// 現在のソート条件
		/// </summary>
		public SortingCondition SortingCondition
		{
			get
			{
				if(source == null) throw new ObjectDisposedException(nameof(source));
				return source.SortingCondition;
			}
		}

		#endregion

		#region Enumerator

		public IEnumerator<T> GetEnumerator()
		{
			if(source == null) throw new ObjectDisposedException(nameof(source));
			return source.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion

		#region INotifyPropertyChanged implementation

		private void Source_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			// 実在するプロパティのみを転送する
			switch(e.PropertyName) {
				case IndexerName:
				case nameof(Count):
				case nameof(SortingCondition):
					RaisePropertyChanged(e);
					break;
			}
		}

		/// <summary>
		/// プロパティ変更時のイベント
		/// </summary>
		public event PropertyChangedEventHandler? PropertyChanged;

		/// <summary>
		/// プロパティが変化したときにこのメソッドを呼ぶ
		/// </summary>
		/// <param name="e">パラメーター</param>
		protected virtual void RaisePropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}

		#endregion

		#region INotifyCollectionChanged implementation

		private void Source_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			RaiseCollectionChanged(e);	// そのまま転送する
		}

		/// <summary>
		/// コレクション変更時のイベント
		/// </summary>
		public event NotifyCollectionChangedEventHandler? CollectionChanged;

		/// <summary>
		/// コレクションが変更通知を発火するメソッド
		/// </summary>
		/// <param name="e">パラメーター</param>
		protected virtual void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			CollectionChanged?.Invoke(this, e);
		}

		#endregion

		#region IDisposable

		private bool disposed;

		protected virtual void Dispose(bool disposing)
		{
			if(!disposed) {
				if(disposing) {
					if(source != null) {
						source.CollectionChanged -= Source_CollectionChanged;
						source.PropertyChanged -= Source_PropertyChanged;
					}
				}
				source = null;

				disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}

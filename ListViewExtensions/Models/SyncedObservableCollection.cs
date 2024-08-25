using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ListViewExtensions.Models
{
	/// <summary>
	/// ObservableCollectionのスレッドセーフ版
	/// </summary>
	/// <typeparam name="T">要素の型</typeparam>
	public class SyncedObservableCollection<T> : IList<T>, ICollection<T>, System.Collections.IList, System.Collections.ICollection, IReadOnlyList<T>, IReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		const string IndexerName = "Item[]";

		protected Synchronizer Sync { get; } = new Synchronizer();
		readonly List<T> items;

		#region Constructor

		/// <summary>
		/// 空のコレクションを生成するコンストラクタ
		/// </summary>
		public SyncedObservableCollection() : this([]) { }

		/// <summary>
		/// 初期アイテムを指定して生成するコンストラクタ
		/// </summary>
		/// <param name="collection">初期アイテム</param>
		public SyncedObservableCollection(IEnumerable<T> collection)
		{
			items = new List<T>(collection);
		}

		#endregion

		#region List premitive functions

		#region Read-Functions

		/// <summary>
		/// 読み取り専用かを取得するプロパティ
		/// 常にfalseです
		/// </summary>
		public bool IsReadOnly => false;

		/// <summary>
		/// コレクションの要素の個数を取得するプロパティ
		/// </summary>
		public int Count
		{
			get => Sync.ReadWithLock(() => items.Count);
		}

		/// <summary>
		/// 固定サイズか
		/// 常にfalse
		/// </summary>
		public bool IsFixedSize { get; } = false;

		/// <summary>
		/// 同期Root
		/// これでlockを取ると、他のスレッドからの昇格可能読み取り操作と書き込み操作がブロックされます。
		/// </summary>
		public virtual object SyncRoot { get => Sync.SyncRoot; }

		/// <summary>
		/// スレッドセーフか
		/// 常にtrue
		/// </summary>
		public virtual bool IsSynchronized { get; } = true;

		/// <summary>
		/// 指定したアイテムが含まれているかを調べるメソッド
		/// </summary>
		/// <param name="item">アイテム</param>
		/// <returns>含まれていればtrue</returns>
		public bool Contains(T item) => Sync.ReadWithLock(() => items.Contains(item));

		/// <summary>
		/// 指定したアイテムが含まれているかを調べるメソッド
		/// </summary>
		/// <param name="value">アイテム</param>
		/// <returns>含まれていればtrue</returns>
		bool System.Collections.IList.Contains(object? value) => Contains((T)(value ?? throw new ArgumentNullException(nameof(value))));

		/// <summary>
		/// 指定したアイテムのインデックスを調べるメソッド
		/// </summary>
		/// <param name="item">アイテム</param>
		/// <returns>インデックス</returns>
		public int IndexOf(T item) => Sync.ReadWithLock(() => items.IndexOf(item));

		/// <summary>
		/// 指定したアイテムのインデックスを調べるメソッド
		/// </summary>
		/// <param name="item">アイテム</param>
		/// <param name="index">検索をスタートするインデックス</param>
		/// <returns>インデックス</returns>
		public int IndexOf(T item, int index) => Sync.ReadWithLock(() => items.IndexOf(item, index));

		/// <summary>
		/// 指定したアイテムのインデックスを調べるメソッド
		/// </summary>
		/// <param name="item">アイテム</param>
		/// <param name="index">検索をスタートするインデックス</param>
		/// <param name="count">検索する個数</param>
		/// <returns>インデックス</returns>
		public int IndexOf(T item, int index, int count) => Sync.ReadWithLock(() => items.IndexOf(item, index, count));

		/// <summary>
		/// 指定したアイテムのインデックスを調べるメソッド
		/// </summary>
		/// <param name="value">アイテム</param>
		/// <returns>インデックス</returns>
		int System.Collections.IList.IndexOf(object? value) => IndexOf((T)(value ?? throw new ArgumentNullException(nameof(value))));

		/// <summary>
		/// 自分自身を指定した配列にコピーするメソッド
		/// </summary>
		/// <param name="array">コピー先</param>
		/// <param name="arrayIndex">コピー先の何番目から書き込むか</param>
		public void CopyTo(T[] array, int arrayIndex) => Sync.ReadWithLock(() => items.CopyTo(array, arrayIndex));

		/// <summary>
		/// 自分自身を指定した配列にコピーするメソッド
		/// </summary>
		/// <param name="array">コピー先</param>
		/// <param name="arrayIndex">コピー先の何番目から書き込むか</param>
		public void CopyTo(Array array, int index) => CopyTo((T[])array, index);

		#endregion

		#region Write-Functions

		#region Indexer

		public T this[int index]
		{
			get
			{
				if(index < 0 || index >= this.Count)
					throw new ArgumentOutOfRangeException(nameof(index));

				return Sync.ReadWithLock(() => GetItem(index));
			}
			set
			{
				Sync.UpgradeableReadWithLock(() => {
					if(index < 0 || index >= this.Count)
						throw new ArgumentOutOfRangeException(nameof(index));

					T oldItem = this[index];

					Sync.WriteWithLock(() => SetItem(index, value));

					RaisePropertyChanged(IndexerName);
					RaiseCollectionReplaced(oldItem, value, index);
				});
			}
		}

		object? System.Collections.IList.this[int index]
		{
			get => this[index];
			set => this[index] = (T)(value ?? throw new ArgumentNullException(nameof(value)));
		}

		protected virtual T GetItem(int index) => items[index];

		protected virtual void SetItem(int index, T item) => items[index] = item;

		#endregion

		#region Add/Insert

		/// <summary>
		/// 要素を追加するメソッド
		/// </summary>
		/// <param name="item">追加するアイテム</param>
		public void Add(T item)
		{
			Sync.UpgradeableReadWithLock(() => Insert(this.Count, item));
		}

		/// <summary>
		/// 要素を追加するメソッド
		/// </summary>
		/// <param name="value">追加するアイテム</param>
		/// <returns>追加された位置</returns>
		int System.Collections.IList.Add(object? value)
		{
			if(value == null) throw new ArgumentNullException(nameof(value));

			return Sync.UpgradeableReadWithLock(() => {
				var index = this.Count;
				Add((T)value);
				return index;
			});
		}

		/// <summary>
		/// 要素を複数同時に追加するメソッド
		/// </summary>
		/// <param name="collection">追加するコレクション</param>
		public void AddRange(IEnumerable<T> collection)
		{
			Sync.UpgradeableReadWithLock(() => InsertRange(this.Count, collection));
		}

		/// <summary>
		/// 要素を挿入するメソッド
		/// </summary>
		/// <param name="index">挿入する位置</param>
		/// <param name="item">挿入するアイテム</param>
		public void Insert(int index, T item)
		{
			InsertRange(index, new T[] { item });
		}

		/// <summary>
		/// 要素を挿入するメソッド
		/// </summary>
		/// <param name="index">挿入する位置</param>
		/// <param name="value">挿入するアイテム</param>
		void System.Collections.IList.Insert(int index, object? value)
		{
			Insert(index, (T)(value ?? throw new ArgumentNullException(nameof(value))));
		}

		/// <summary>
		/// 要素を複数同時に挿入するメソッド
		/// </summary>
		/// <param name="index">挿入する位置</param>
		/// <param name="collection">挿入するコレクション</param>
		public void InsertRange(int index, IEnumerable<T> collection)
		{
			if(collection == null)
				throw new ArgumentNullException(nameof(collection));

			var array = collection.ToArray();

			if(array.Length == 0)
				return;		//追加しない

			Sync.UpgradeableReadWithLock(() => {
				if(index < 0 || index > this.Count)
					throw new ArgumentOutOfRangeException(nameof(index));

				Sync.WriteWithLock(() => InsertItem(index, array));

				RaisePropertyChanged(nameof(Count));
				RaisePropertyChanged(IndexerName);
				RaiseCollectionInserted(array, index);
			});
		}

		protected virtual void InsertItem(int index, T[] array)
		{
			items.InsertRange(index, array);
		}

		#endregion

		#region Remove

		/// <summary>
		/// 指定のアイテムを削除するメソッド
		/// </summary>
		/// <param name="item">削除したいアイテム</param>
		/// <returns>削除したらtrue、しなかったらfalse</returns>
		public bool Remove(T item)
		{
			return Sync.UpgradeableReadWithLock(() => {
				int index = IndexOf(item);
				if(index < 0)
					return false;
				RemoveAt(index);
				return true;
			});
		}

		/// <summary>
		/// 指定のアイテムを削除するメソッド
		/// </summary>
		/// <param name="value">削除したいアイテム</param>
		void System.Collections.IList.Remove(object? value)
		{
			Remove((T)(value ?? throw new ArgumentNullException(nameof(value))));
		}

		/// <summary>
		/// 指定の位置のアイテムを削除するメソッド
		/// </summary>
		/// <param name="index">削除する位置</param>
		public void RemoveAt(int index)
		{
			RemoveRange(index, 1);
		}

		/// <summary>
		/// 指定の位置のアイテムを削除するメソッド
		/// </summary>
		/// <param name="index">削除開始位置</param>
		/// <param name="count">削除する個数</param>
		public void RemoveRange(int index, int count)
		{
			if(count < 0)
				throw new ArgumentOutOfRangeException(nameof(count));
			if(count == 0)
				return;		//削除しない

			Sync.UpgradeableReadWithLock(() => {
				if(index < 0 || (index + count) > this.Count)
					throw new ArgumentOutOfRangeException(nameof(index));

				T[] removedItems = new T[count];
				for(int i = 0; i < count; i++)
					removedItems[i] = this[index + i];

				Sync.WriteWithLock(() => RemoveItem(index, count));

				RaisePropertyChanged(nameof(Count));
				RaisePropertyChanged(IndexerName);
				RaiseCollectionRemoved(removedItems, index);
			});
		}

		protected virtual void RemoveItem(int index, int count)
		{
			items.RemoveRange(index, count);
		}

		#endregion

		#region Move

		/// <summary>
		/// アイテムを移動するメソッド
		/// </summary>
		/// <param name="oldIndex">古い位置</param>
		/// <param name="newIndex">新しい位置</param>
		public void Move(int oldIndex, int newIndex)
		{
			MoveRange(oldIndex, newIndex, 1);
		}

		/// <summary>
		/// アイテムを移動するメソッド
		/// </summary>
		/// <param name="oldIndex">古い位置</param>
		/// <param name="newIndex">新しい位置</param>
		/// <param name="count">個数</param>
		public void MoveRange(int oldIndex, int newIndex, int count)
		{
			if(count < 0)
				throw new ArgumentOutOfRangeException(nameof(count));
			if(count == 0)
				return;	//移動しない

			Sync.UpgradeableReadWithLock(() => {
				if(oldIndex < 0 || (oldIndex + count) > this.Count)
					throw new ArgumentOutOfRangeException(nameof(oldIndex));
				if(newIndex < 0 || (newIndex + count) > this.Count)
					throw new ArgumentOutOfRangeException(nameof(newIndex));

				T[] movingItems = new T[count];
				for(int i = 0; i < count; i++)
					movingItems[i] = this[oldIndex + i];

				Sync.WriteWithLock(() => {
					MoveItem(oldIndex, newIndex, count);
				});

				RaisePropertyChanged(IndexerName);
				RaiseCollectionMoved(movingItems, newIndex, oldIndex);
			});
		}

		protected virtual void MoveItem(int oldIndex, int newIndex, int count)
		{
			T[] movingItems = new T[count];
			for(int i = 0; i < count; i++)
				movingItems[i] = this[oldIndex + i];

			items.RemoveRange(oldIndex, count);
			items.InsertRange(newIndex, movingItems);
		}

		#endregion

		#region Clear

		/// <summary>
		/// アイテムを全削除するメソッド
		/// </summary>
		public void Clear()
		{
			Sync.UpgradeableReadWithLock(() => {
				Sync.WriteWithLock(() => ClearItems());

				RaisePropertyChanged(nameof(Count));
				RaisePropertyChanged(IndexerName);
				RaiseCollectionReset();
			});
		}

		protected virtual void ClearItems()
		{
			items.Clear();
		}

		#endregion

		#endregion

		#region Enumerator

		public IEnumerator<T> GetEnumerator() => Sync.ReadWithLock(() => items.GetEnumerator());

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion

		#endregion

		#region INotifyPropertyChanged implementation

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

		/// <summary>
		/// プロパティが変化したときにこのメソッドを呼ぶ
		/// </summary>
		/// <param name="PropertyName">変化したプロパティ名</param>
		protected void RaisePropertyChanged(string PropertyName)
		{
			RaisePropertyChanged(new PropertyChangedEventArgs(PropertyName));
		}

		#endregion

		#region INotifyCollectionChanged implementation

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

		/// <summary>
		/// コレクションが追加されたときに変更通知を発火するメソッド
		/// </summary>
		/// <param name="item">追加されたアイテム</param>
		/// <param name="index">追加された場所</param>
		protected void RaiseCollectionInserted(T item, int index)
		{
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
		}

		/// <summary>
		/// コレクションが追加されたときに変更通知を発火するメソッド
		/// </summary>
		/// <param name="items">追加されたアイテム</param>
		/// <param name="index">追加された場所</param>
		protected void RaiseCollectionInserted(IEnumerable<T> items, int index)
		{
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items.ToList(), index));
		}

		/// <summary>
		/// コレクションが削除されたときに変更通知を発火するメソッド
		/// </summary>
		/// <param name="item">削除されたアイテム</param>
		/// <param name="index">削除された場所</param>
		protected void RaiseCollectionRemoved(T item, int index)
		{
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
		}

		/// <summary>
		/// コレクションが削除されたときに変更通知を発火するメソッド
		/// </summary>
		/// <param name="items">削除されたアイテム</param>
		/// <param name="index">削除された場所</param>
		protected void RaiseCollectionRemoved(IEnumerable<T> items, int index)
		{
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items.ToList(), index));
		}

		/// <summary>
		/// コレクションの中身が置き換えられたときに変更通知を発火するメソッド
		/// </summary>
		/// <param name="oldItem">古いアイテム</param>
		/// <param name="newItem">新しいアイテム</param>
		/// <param name="index">インデックス</param>
		protected void RaiseCollectionReplaced(T oldItem, T newItem, int index)
		{
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
		}

		/// <summary>
		/// コレクションの中身が移動されたときに変更通知を発火するメソッド
		/// </summary>
		/// <param name="item">移動したアイテム</param>
		/// <param name="index">新しいインデックス</param>
		/// <param name="oldIndex">古いインデックス</param>
		protected void RaiseCollectionMoved(T item, int index, int oldIndex)
		{
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, index, oldIndex));
		}

		/// <summary>
		/// コレクションの中身が移動されたときに変更通知を発火するメソッド
		/// </summary>
		/// <param name="items">移動したアイテム</param>
		/// <param name="index">新しいインデックス</param>
		/// <param name="oldIndex">古いインデックス</param>
		protected void RaiseCollectionMoved(IEnumerable<T> items, int index, int oldIndex)
		{
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, items.ToList(), index, oldIndex));
		}

		/// <summary>
		/// コレクションが大幅に変わったかゼロになった
		/// </summary>
		protected void RaiseCollectionReset()
		{
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		#endregion
	}
}

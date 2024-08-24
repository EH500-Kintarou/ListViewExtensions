using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ListViewExtensions.Models
{
	public class Synchronizer
	{
		private ReaderWriterLockSlim rwlock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

		/// <summary>
		/// UpgradeableReadLock及びWriteLockするときにこのオブジェクトが使われます。
		/// </summary>
		public object SyncRoot { get; } = new object();

		#region Read

		/// <summary>
		/// スレッドセーフにデータを読み込むメソッド
		/// </summary>
		/// <typeparam name="TResult">読み込む型</typeparam>
		/// <param name="ReadAction">実際の読み込み部</param>
		public void ReadWithLock(Action ReadAction)
		{
			try {
				rwlock.EnterReadLock();
				ReadAction();
			}
			finally {
				rwlock.ExitReadLock();
			}
		}

		/// <summary>
		/// スレッドセーフにデータを読み込むメソッド
		/// </summary>
		/// <typeparam name="TResult">読み込む型</typeparam>
		/// <param name="ReadAction">実際の読み込み部</param>
		/// <returns>読み込んだデータ</returns>
		public TResult ReadWithLock<TResult>(Func<TResult> ReadAction)
		{
			try {
				rwlock.EnterReadLock();
				return ReadAction();
			}
			finally {
				rwlock.ExitReadLock();
			}
		}

		#endregion

		#region UpgradeableRead

		/// <summary>
		/// スレッドセーフにデータを読み込む（昇格可能）メソッド
		/// </summary>
		/// <param name="UpgradeableReadAction">実際の読み込み部</param>
		public void UpgradeableReadWithLock(Action UpgradeableReadAction)
		{
			lock(SyncRoot) {
				try {
					rwlock.EnterUpgradeableReadLock();
					UpgradeableReadAction();
				}
				finally {
					rwlock.ExitUpgradeableReadLock();
				}
			}
		}

		/// <summary>
		/// スレッドセーフにデータを読み込む（昇格可能）メソッド
		/// </summary>
		/// <typeparam name="TResult">読み込む型</typeparam>
		/// <param name="UpgradeableReadAction">実際の読み込み部</param>
		/// <returns>読み込んだデータ</returns>
		public TResult UpgradeableReadWithLock<TResult>(Func<TResult> UpgradeableReadAction)
		{
			lock(SyncRoot) {
				try {
					rwlock.EnterUpgradeableReadLock();
					return UpgradeableReadAction();
				}
				finally {
					rwlock.ExitUpgradeableReadLock();
				}
			}
		}

		#endregion

		#region Write

		/// <summary>
		/// スレッドセーフにデータを書き込むメソッド
		/// </summary>
		/// <param name="WriteAction">その読み込まれた値を渡される書き込み部</param>
		public void WriteWithLock(Action WriteAction)
		{
			lock(SyncRoot) {
				try {
					rwlock.EnterWriteLock();
					WriteAction();
				}
				finally {
					rwlock.ExitWriteLock();
				}
			}
		}

		/// <summary>
		/// スレッドセーフにデータを書き込むメソッド
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="WriteAction">その読み込まれた値を渡される書き込み部</param>
		/// <returns>WriteActionの返却値</returns>
		public TResult WriteWithLock<TResult>(Func<TResult> WriteAction)
		{
			lock(SyncRoot) {
				try {
					rwlock.EnterWriteLock();
					return WriteAction();
				}
				finally {
					rwlock.ExitWriteLock();
				}
			}
		}

		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListViewExtensions
{
	public struct SortingCondition : IEquatable<SortingCondition>
	{
		/// <summary>
		/// ソート条件なしとして初期化します
		/// </summary>
		public SortingCondition() : this(SortingDirection.None) { }

		/// <summary>
		/// 要素そのものをキーとしてソート条件を指定し初期化します。
		/// </summary>
		/// <param name="direction"></param>
		public SortingCondition(SortingDirection direction)
		{
			PropertyName = null;
			Direction = direction;
		}

		/// <summary>
		/// ソート条件を指定して初期化します
		/// </summary>
		/// <param name="propertyName">ソートに使うプロパティ名</param>
		/// <param name="direction">ソート方向</param>
		public SortingCondition(string? propertyName, SortingDirection direction)
		{
			if(direction == SortingDirection.None)
				PropertyName = null;
			else
				PropertyName = propertyName;

			Direction = direction;
		}

		/// <summary>
		/// プロパティ名
		/// </summary>
		public string? PropertyName { get; }

		/// <summary>
		/// ソート方向
		/// </summary>
		public SortingDirection Direction { get; }

		/// <summary>
		/// ソート条件がなしかを取得します。
		/// </summary>
		public bool IsNone => Direction == SortingDirection.None;

		#region IEquatable

		public override bool Equals(object? obj)
		{
			return obj != null && base.Equals((SortingCondition)obj);
		}

		public override int GetHashCode()
		{
			var hash = Direction.GetHashCode();
			if(PropertyName != null)
				hash ^= PropertyName.GetHashCode();

			return hash;
		}

#if(NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER)
		public bool Equals([AllowNull] SortingCondition other)
#else
		public bool Equals(SortingCondition other)
#endif
		{
			return this.PropertyName == other.PropertyName && this.Direction == other.Direction;
		}

		public static bool operator ==(SortingCondition left, SortingCondition right) => left.Equals(right);
		public static bool operator !=(SortingCondition left, SortingCondition right) => !left.Equals(right);

		#endregion

		public override string ToString()
		{
			if(IsNone)
				return "None";
			else
				return $"{PropertyName}, {Direction}";
        }

		/// <summary>
		/// ソート条件なし
		/// </summary>
		public readonly static SortingCondition None = new SortingCondition();
	}
}

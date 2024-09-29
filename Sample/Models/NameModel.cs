using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Models
{
	public class NameModel : NotificationObject
	{
		#region Spell変更通知プロパティ

		public string? Spell
		{
			get => _Spell;
			set => RaisePropertyChangedIfSet(ref _Spell, value);
		}
		private string? _Spell;

		#endregion

		#region Pronunciation変更通知プロパティ

		public string? Pronunciation
		{
			get => _Pronunciation;
			set => RaisePropertyChangedIfSet(ref _Pronunciation, value);
		}
		private string? _Pronunciation;

		#endregion
	}
}

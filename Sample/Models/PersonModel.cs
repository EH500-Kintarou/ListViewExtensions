using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;

namespace Sample.Models
{
	public class PersonModel : NotificationObject
	{
		#region Name変更通知プロパティ

		public NameModel? Name
		{
			get => _Name;
			set => RaisePropertyChangedIfSet(ref _Name, value);
		}
		private NameModel? _Name;

		#endregion

		#region Pronunciation変更通知プロパティ

		public string? Pronunciation
		{
			get => _Pronunciation;
			set => RaisePropertyChangedIfSet(ref _Pronunciation, value);
		}
		private string? _Pronunciation;

		#endregion

		#region Age変更通知プロパティ

		public int Age
		{
			get => _Age;
			set => RaisePropertyChangedIfSet(ref _Age, value);
		}
		private int _Age;

		#endregion

		#region Birthday変更通知プロパティ

		public DateTime Birthday
		{
			get => _Birthday;
			set => RaisePropertyChangedIfSet(ref _Birthday, value);
		}
		private DateTime _Birthday;

		#endregion
		
		#region Height_cm変更通知プロパティ

		public int Height_cm
		{
			get => _Height_cm;
			set => RaisePropertyChangedIfSet(ref _Height_cm, value);
		}
		private int _Height_cm;

		#endregion
		
		public void IncrementAge()
		{
			Age++;
			Birthday = new DateTime(Birthday.Year - 1, Birthday.Month, Birthday.Day);
		}

		public void DecrementAge()
		{
			Age--;
			Birthday = new DateTime(Birthday.Year + 1, Birthday.Month, Birthday.Day);
		}
	}
}

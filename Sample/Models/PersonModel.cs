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

		public string? Name
		{
			get { return _Name; }
			set
			{
				if(_Name == value)
					return;
				_Name = value;
				RaisePropertyChanged(nameof(Name));
			}
		}
		private string? _Name;

		#endregion

		#region Pronunciation変更通知プロパティ

		public string? Pronunciation
		{
			get { return _Pronunciation; }
			set
			{
				if(_Pronunciation == value)
					return;
				_Pronunciation = value;
				RaisePropertyChanged(nameof(Pronunciation));
			}
		}
		private string? _Pronunciation;

		#endregion

		#region Age変更通知プロパティ

		public int Age
		{
			get { return _Age; }
			set
			{
				if(_Age == value)
					return;
				_Age = value;
				RaisePropertyChanged(nameof(Age));
			}
		}
		private int _Age;

		#endregion

		#region Birthday変更通知プロパティ

		public DateTime Birthday
		{
			get { return _Birthday; }
			set
			{
				if(_Birthday == value)
					return;
				_Birthday = value;
				RaisePropertyChanged(nameof(Birthday));
			}
		}
		private DateTime _Birthday;

		#endregion
		
		#region Height_cm変更通知プロパティ

		public int Height_cm
		{
			get { return _Height_cm; }
			set
			{ 
				if(_Height_cm == value)
					return;
				_Height_cm = value;
				RaisePropertyChanged(nameof(Height_cm));
			}
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

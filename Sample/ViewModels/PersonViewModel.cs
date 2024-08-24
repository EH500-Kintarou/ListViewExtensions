using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using Sample.Models;

namespace Sample.ViewModels
{
	public class PersonViewModel : ViewModel
	{
		Person Source;

		public PersonViewModel(Person source)
		{
			Source = source;
			Source.PropertyChanged += Source_PropertyChanged;
		}

		private void Source_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName) {
				case nameof(Source.Name):
					RaisePropertyChanged(nameof(this.Name));
					break;
				case nameof(Source.Pronunciation):
					RaisePropertyChanged(nameof(this.Pronunciation));
					break;
				case nameof(Source.Age):
					RaisePropertyChanged(nameof(this.Age));
					break;
				case nameof(Source.Birthday):
					RaisePropertyChanged(nameof(this.Birthday));
					break;
				case nameof(Source.Height_cm):
					RaisePropertyChanged(nameof(this.Height));
					break;
			}
		}

		public void Initialize()
		{
		}

		public string Name
		{
			get { return Source.Name; }
			set { Source.Name = value; }
		}

		public string Pronunciation
		{
			get { return Source.Pronunciation; }
			set { Source.Pronunciation = value; }
		}

		public string Age
		{
			get { return $"{Source.Age}歳"; }
			//set { Source.Age = value; }
		}

		public string Birthday
		{
			get { return Source.Birthday.ToShortDateString(); }
			//set { Source.Birthday = value; }
		}

		public string Height
		{
			get { return $"{Source.Height_cm}cm"; }
			//set { Source.Height_cm = value; }
		}

		#region IncrementAgeCommand

		public ViewModelCommand IncrementAgeCommand
		{
			get
			{
				if(_IncrementAgeCommand == null)
					_IncrementAgeCommand = new ViewModelCommand(() => Source.IncrementAge());
				return _IncrementAgeCommand;
			}
		}
		private ViewModelCommand _IncrementAgeCommand;

		#endregion

		#region DecrementAgeCommand

		public ViewModelCommand DecrementAgeCommand
		{
			get
			{
				if(_DecrementAgeCommand == null)
					_DecrementAgeCommand = new ViewModelCommand(() => Source.DecrementAge());
				return _DecrementAgeCommand;
			}
		}
		private ViewModelCommand _DecrementAgeCommand;

		#endregion

		public void DoubleClicked()
		{
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
				Source.PropertyChanged -= Source_PropertyChanged;
			
			base.Dispose(disposing);
		}
	}
}

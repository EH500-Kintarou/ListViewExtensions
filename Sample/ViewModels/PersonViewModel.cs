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
		readonly PersonModel model;

		public PersonViewModel(PersonModel model)
		{
			this.model = model;

			if(model.Name != null) {
				Name = new NameViewModel(model.Name);
				Name.PropertyChanged += Name_PropertyChanged;
			}

			model.PropertyChanged += Model_PropertyChanged;
		}

		private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName) {
				case nameof(Name):
					if(Name != null)
						Name.PropertyChanged -= Name_PropertyChanged;
					Name = null;
					if(model.Name != null) {
						Name = new NameViewModel(model.Name);
						Name.PropertyChanged += Name_PropertyChanged;
					}
					break;
				case nameof(model.Age):
					RaisePropertyChanged(nameof(Age));
					break;
				case nameof(model.Birthday):
					RaisePropertyChanged(nameof(Birthday));
					break;
				case nameof(model.Height_cm):
					RaisePropertyChanged(nameof(Height));
					break;
			}
		}

		private void Name_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			throw new NotImplementedException();
		}

		public NameViewModel? Name
		{
			get => _Name;
			set => RaisePropertyChangedIfSet(ref _Name, value);
		}
		NameViewModel? _Name;

		public string Age => $"{model.Age}歳";

		public string Birthday => model.Birthday.ToShortDateString();

		public string Height => $"{model.Height_cm}cm";

		#region IncrementAgeCommand

		public ViewModelCommand IncrementAgeCommand
		{
			get
			{
				_IncrementAgeCommand ??= new ViewModelCommand(model.IncrementAge);
				return _IncrementAgeCommand;
			}
		}
		private ViewModelCommand? _IncrementAgeCommand;

		#endregion

		#region DecrementAgeCommand

		public ViewModelCommand DecrementAgeCommand
		{
			get
			{
				_DecrementAgeCommand ??= new ViewModelCommand(model.DecrementAge);
				return _DecrementAgeCommand;
			}
		}
		private ViewModelCommand? _DecrementAgeCommand;

		#endregion

		public void DoubleClicked()
		{
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
				model.PropertyChanged -= Model_PropertyChanged;
			
			base.Dispose(disposing);
		}
	}
}

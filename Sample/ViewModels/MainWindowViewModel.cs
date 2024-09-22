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
using ListViewExtensions;
using ListViewExtensions.ViewModels;

namespace Sample.ViewModels
{
	public class MainWindowViewModel : ViewModel
	{
		Random random = new Random();
		MainWindowModel? model;

		public void Initialize()
		{
			model = MainWindowModel.GetInstance();

			People = new ListViewViewModel<PersonViewModel, PersonModel>(model.People, person => new PersonViewModel(person), new Dictionary<string, string>() { { nameof(PersonModel.Height_cm), nameof(PersonViewModel.Height) } }, DispatcherHelper.UIDispatcher);
		}

		public void Add()
		{
			model?.Add();
		}

		public void ClearAll()
		{
			model?.ClearAll();
		}

		#region People変更通知プロパティ

		public ListViewViewModel<PersonViewModel, PersonModel>? People
		{
			get { return _People; }
			set
			{
				if(_People == value)
					return;
				_People = value;
				RaisePropertyChanged(nameof(People));
			}
		}
		private ListViewViewModel<PersonViewModel, PersonModel>? _People;

		#endregion
	}
}

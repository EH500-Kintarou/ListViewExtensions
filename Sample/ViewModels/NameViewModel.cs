using Livet;
using Sample.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.ViewModels
{
	public class NameViewModel : ViewModel
	{
		readonly NameModel model;

		public NameViewModel(NameModel model)
		{
			this.model = model;
			model.PropertyChanged += Model_PropertyChanged;
		}

		private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName) {
				case nameof(model.Spell):
					RaisePropertyChanged(nameof(Spell));
					break;
				case nameof(model.Pronunciation):
					RaisePropertyChanged(nameof(Pronunciation));
					break;
			}
		}

		#region Spell変更通知プロパティ

		public string? Spell
		{
			get => model.Spell;
			set => model.Spell = value;
		}

		#endregion

		#region Pronunciation変更通知プロパティ

		public string? Pronunciation
		{
			get => model.Pronunciation;
			set => model.Pronunciation = value;
		}

		#endregion

		protected override void Dispose(bool disposing)
		{
			if(disposing)
				model.PropertyChanged -= Model_PropertyChanged;

			base.Dispose(disposing);

		}
	}
}

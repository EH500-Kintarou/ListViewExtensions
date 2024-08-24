using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ListViewExtensions.ViewModels.Commands
{
	internal class Command : ICommand
	{
		readonly Func<bool> CanExecuteFunc;
		readonly Action ExecuteAction;

		public Command(Action ExecuteAction) : this(() => true, ExecuteAction) { }

		public Command(Func<bool> CanExecuteFunc, Action ExecuteAction)
		{
			this.CanExecuteFunc = CanExecuteFunc;
			this.ExecuteAction = ExecuteAction;
		}

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, new EventArgs());
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter) => CanExecuteFunc();

		public void Execute(object parameter) => ExecuteAction();
	}
}

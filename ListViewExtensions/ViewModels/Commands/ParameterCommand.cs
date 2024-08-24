using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ListViewExtensions.ViewModels.Commands
{
	internal class ParameterCommand<TParam> : ICommand
	{
		readonly Func<TParam?, bool> CanExecuteFunc;
		readonly Action<TParam?> ExecuteAction;

		public ParameterCommand(Action<TParam?> ExecuteAction) : this((p) => true, ExecuteAction) { }

		public ParameterCommand(Func<TParam?, bool> CanExecuteFunc, Action<TParam?> ExecuteAction)
		{
			this.CanExecuteFunc = CanExecuteFunc;
			this.ExecuteAction = ExecuteAction;
		}

		public void RaiseCanExecuteChanged()
		{
			if(CanExecuteChanged != null)
				CanExecuteChanged(this, new EventArgs());
		}

		public event EventHandler? CanExecuteChanged;

		public bool CanExecute(object? parameter)
		{
			if(parameter == null)
				return CanExecuteFunc(default(TParam));
			else
				return CanExecuteFunc((TParam)parameter);
		}

		public void Execute(object? parameter)
		{
			if(parameter == null)
				ExecuteAction(default(TParam));
			else
				ExecuteAction((TParam)parameter);
		}
	}
}

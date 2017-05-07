using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MatchingMakingMonitor.ViewModels
{
	public class RelayCommand : ICommand
	{
		protected Action<object> actionWithParam;
		protected Action action;

		public RelayCommand(Action<object> action)
		{
			this.actionWithParam = action;
		}
		public RelayCommand(Action action)
		{
			this.action = action;
		}

		public event EventHandler CanExecuteChanged;

		private bool canExecute = true;

		public bool CanExecute(object parameter)
		{
			return canExecute;
		}

		public void Execute(object parameter)
		{
			action?.Invoke();
			actionWithParam?.Invoke(parameter);
		}
	}
}

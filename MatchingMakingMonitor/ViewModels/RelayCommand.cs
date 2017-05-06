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
		private Action action;

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
		}
	}
}

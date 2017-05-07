using MatchingMakingMonitor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MatchingMakingMonitor.ViewModels
{
	public class SettingsWindowViewModel : BaseViewBinding
	{
		public RelayCommand ResetCommand { get; set; }

		public ObservableCollection<int> FontSizes { get; private set; } = new ObservableCollection<int>() { 8, 9, 10, 11, 12, 13, 14 };

		private LoggingService loggingService;
		public Settings Settings { get; set; }
		public SettingsWindowViewModel(LoggingService loggingService, Services.Settings Settings)
		{
			this.loggingService = loggingService;
			this.Settings = Settings;

			this.ResetCommand = new RelayCommand(() =>
			{
				var result = MessageBox.Show("Are you sure you want to reset all Settings?", "Reset Settings", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes)
				{
					this.Settings.ResetUI();
				}
			});
		}

		public SettingsWindowViewModel()
		{

		}
	}
}

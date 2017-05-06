using MatchingMakingMonitor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingMakingMonitor.ViewModels
{
	public class HeaderViewModel : BaseViewModel
	{
		public RelayCommand LogoClickCommand { get; set; }
		public RelayCommand SettingsCommand { get; set; }

		private LoggingService loggingService;
		public HeaderViewModel()
		{

		}

		public HeaderViewModel(LoggingService loggingService)
		{
			this.loggingService = loggingService;
			this.LogoClickCommand = new RelayCommand(logoClick);
			this.SettingsCommand = new RelayCommand(settingsClick);
		}

		private void logoClick()
		{
			try
			{
				System.Diagnostics.Process.Start("https://wowreplays.com");
			} //end try
			catch (Exception ex)
			{
				loggingService.Log("Exception Throw on Logo Click: " + ex.Message);
			} //end catch
		}

		private void settingsClick()
		{
			var settingsWindow = IoCKernel.Get<Settings>();
			settingsWindow.Show();
		}
	}
}

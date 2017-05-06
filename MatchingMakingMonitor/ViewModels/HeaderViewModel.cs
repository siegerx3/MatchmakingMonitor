using MatchingMakingMonitor.Services;
using MatchingMakingMonitor.SocketIO;
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

		private string connectionState;

		public string ConnectionState
		{
			get { return connectionState; }
			set
			{
				connectionState = value;
				FirePropertyChanged();
			}
		}


		private LoggingService loggingService;
		private SocketIOService socketIOService;
		public HeaderViewModel()
		{

		}

		public HeaderViewModel(LoggingService loggingService, SocketIOService socketIOService)
		{
			this.loggingService = loggingService;
			this.socketIOService = socketIOService;

			this.LogoClickCommand = new RelayCommand(logoClick);
			this.SettingsCommand = new RelayCommand(settingsClick);

			this.socketIOService.StateChanged.Subscribe(state =>
			{
				ConnectionState = state.ToString();
			});
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

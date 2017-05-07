using MatchingMakingMonitor.Services;
using MatchingMakingMonitor.SocketIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchingMakingMonitor.ViewModels
{
	public class HeaderViewModel : BaseViewBinding
	{
		public RelayCommand LogoClickCommand { get; set; }
		public RelayCommand SettingsCommand { get; set; }

		private string connectionState;

		private SettingsWindow settingsWindow;

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
			}
			catch (Exception e)
			{
				loggingService.Error("Exception Throw on Logo Click", e);
			}
		}

		private void settingsClick()
		{
			if(settingsWindow == null)
			{
				settingsWindow = IoCKernel.Get<SettingsWindow>();
				settingsWindow.Show();
				settingsWindow.Closed += (sender, args) =>
				{
					settingsWindow = null;
				};
			}
		}
	}
}

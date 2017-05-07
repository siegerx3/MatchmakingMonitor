using System;
using MatchMakingMonitor.Services;
using MatchMakingMonitor.SocketIO;

namespace MatchMakingMonitor.View
{
	public class HeaderViewModel : BaseViewBinding
	{
		public RelayCommand LogoClickCommand { get; set; }
		public RelayCommand SettingsCommand { get; set; }

		private string _connectionState;

		private SettingsWindow _settingsWindow;

		public string ConnectionState
		{
			get => _connectionState;
			set
			{
				_connectionState = value;
				FirePropertyChanged();
			}
		}


		private readonly LoggingService _loggingService;

		public HeaderViewModel()
		{

		}

		public HeaderViewModel(LoggingService loggingService, SocketIoService socketIoService)
		{
			_loggingService = loggingService;

			LogoClickCommand = new RelayCommand(LogoClick);
			SettingsCommand = new RelayCommand(SettingsClick);

			socketIoService.StateChanged.Subscribe(state =>
			{
				ConnectionState = state.ToString();
			});
		}

		private void LogoClick()
		{
			try
			{
				System.Diagnostics.Process.Start("https://wowreplays.com");
			}
			catch (Exception e)
			{
				_loggingService.Error("Exception Throw on Logo Click", e);
			}
		}

		private void SettingsClick()
		{
			if (_settingsWindow != null) return;
			_settingsWindow = IoCKernel.Get<SettingsWindow>();
			_settingsWindow.Show();
			_settingsWindow.Closed += (sender, args) =>
			{
				_settingsWindow = null;
			};
		}
	}
}

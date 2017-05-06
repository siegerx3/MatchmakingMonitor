using MatchingMakingMonitor.Services;
using MatchingMakingMonitor.SocketIO;
using MatchingMakingMonitor.ViewModels;
using Ninject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MatchingMakingMonitor
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private SocketIOService socketIOService;
		protected override void OnStartup(StartupEventArgs e)
		{
			IoCKernel.Init();
			ConfigureMainWindow();

			//window.DataContext = new MainWindowViewModel();
			//window.Header.DataContext = new HeaderViewModel();

			Current.MainWindow.Show();

			socketIOService = IoCKernel.Get<SocketIOService>();
			var ss = IoCKernel.Get<StatsService>();
			socketIOService.Connect();
			socketIOService.StateChanged.Where(s => s == ConnectionState.Connected).Subscribe(_ =>
			{
				if (!string.IsNullOrEmpty(MatchingMakingMonitor.Properties.Settings.Default.Token))
				{
					socketIOService.Hub.SetToken(MatchingMakingMonitor.Properties.Settings.Default.Token);
				}
			});
			base.OnStartup(e);
		}

		private void ConfigureMainWindow()
		{
			Current.MainWindow = IoCKernel.Get<MainWindow>();
		}



		protected override void OnExit(ExitEventArgs e)
		{
			socketIOService.Disconnect();
			base.OnExit(e);
		}
	}
}

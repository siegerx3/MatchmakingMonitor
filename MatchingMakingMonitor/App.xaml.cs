using MatchMakingMonitor.Services;
using MatchMakingMonitor.SocketIO;
using MatchMakingMonitor.ViewModels;
using Ninject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MatchMakingMonitor
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

			Current.MainWindow.Show();

			//socketIOService = IoCKernel.Get<SocketIOService>();
			//socketIOService.Connect();
			//socketIOService.StateChanged.Where(s => s == ConnectionState.Connected).Subscribe(_ =>
			//{
			//	if (!string.IsNullOrEmpty(MatchMakingMonitor.Properties.Settings.Default.Token))
			//	{
			//		socketIOService.Hub.SetToken(MatchMakingMonitor.Properties.Settings.Default.Token);
			//	}
			//});
			base.OnStartup(e);
		}

		private void ConfigureMainWindow()
		{
			Current.MainWindow = IoCKernel.Get<MainWindow>();
		}



		protected override void OnExit(ExitEventArgs e)
		{
			//socketIOService.Disconnect();
			base.OnExit(e);
		}
	}
}

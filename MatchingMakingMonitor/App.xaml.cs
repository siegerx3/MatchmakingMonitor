using MatchMakingMonitor.Services;
using System.Windows;

namespace MatchMakingMonitor
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
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

		private static void ConfigureMainWindow()
		{
			Current.MainWindow = IoCKernel.Get<MainWindow>();
		}
	}
}

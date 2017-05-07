using MatchMakingMonitor.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace MatchMakingMonitor.ViewModels
{
	public class SubHeaderViewModel : BaseViewBinding
	{
		public RelayCommand PathClickCommand { get; set; }

		public ObservableCollection<string> Regions { get; private set; } = new ObservableCollection<string>() { "NA", "EU", "RU", "SEA" };

		private string region = "NA";

		public string Region
		{
			get { return region; }
			set
			{
				region = value;
				settings.Region = value;
				FirePropertyChanged();
			}
		}


		private string installDirectoryText = "Check install directory";

		public string InstallDirectoryText
		{
			get { return installDirectoryText; }
			set
			{
				installDirectoryText = value;
				FirePropertyChanged();
			}
		}

		private SolidColorBrush installDirectoryColor;

		public SolidColorBrush InstallDirectoryColor
		{
			get { return installDirectoryColor; }
			set
			{
				installDirectoryColor = value;
				FirePropertyChanged();
			}
		}

		private string statusText = "Not currently in Battle...";

		public string StatusText
		{
			get { return statusText; }
			set
			{
				statusText = value;
				FirePropertyChanged();
			}
		}

		private SolidColorBrush statusColor = Brushes.Black;

		public SolidColorBrush StatusColor
		{
			get { return statusColor; }
			set
			{
				statusColor = value;
				FirePropertyChanged();
			}
		}

		private bool enableUI = true;
		public bool EnableUI
		{
			get { return enableUI; }
			set
			{
				enableUI = value;
				FirePropertyChanged();
			}
		}

		private Visibility showProgress = Visibility.Collapsed;
		public Visibility ShowProgress
		{
			get { return showProgress; }
			set
			{
				showProgress = value;
				FirePropertyChanged();
			}
		}

		private LoggingService loggingService;
		private Services.Settings settings;
		private StatsService statsService;

		public SubHeaderViewModel(Services.Settings settings, LoggingService loggingService, StatsService statsService)
		{
			this.settings = settings;
			this.loggingService = loggingService;
			this.statsService = statsService;

			this.PathClickCommand = new RelayCommand(pathClicked);

			this.settings.SettingChanged(Settings.KeyInstallDirectory).Subscribe(initPath);
			this.statsService.StatsStatusChanged.Subscribe(status =>
			{
				setStatusText(status);
				if (status == StatsStatus.Fetching)
				{
					EnableUI = false;
					ShowProgress = Visibility.Visible;
				}
				else
				{
					EnableUI = true;
					ShowProgress = Visibility.Collapsed;
				}
			});


			Region = this.settings.Region;
		}

		public SubHeaderViewModel()
		{
		}

		private void initPath(string key)
		{
			var directory = settings.Get<string>(key);
			if (Directory.Exists(Path.Combine(directory, "replays")) && File.Exists(Path.Combine(directory, "WorldOfWarships.exe")))
			{
				InstallDirectoryColor = Brushes.Green;
				InstallDirectoryText = directory;
			}
			else
			{
				InstallDirectoryColor = Brushes.OrangeRed;
				InstallDirectoryText = directory + " - Invalid Path or Replays not enabled! - Click here to update!";
			}
		}

		private void pathClicked()
		{
			var folderBrowser = new FolderBrowserDialog();
			var result = folderBrowser.ShowDialog();
			if (result == DialogResult.OK && !string.IsNullOrEmpty(folderBrowser.SelectedPath))
			{
				this.settings.InstallDirectory = folderBrowser.SelectedPath;
			}
		}

		private void setStatusText(StatsStatus status)
		{
			switch (status)
			{
				case StatsStatus.Fetching:
					StatusText = "Fetching player stats for current battle";
					StatusColor = Brushes.DarkOrange;
					break;
				case StatsStatus.Fetched:
					StatusText = "Player Stats Succesfully Updated";
					StatusColor = Brushes.Green;
					break;
				case StatsStatus.WrongRegion:
					StatusText = "Found a battle but no stats. Wrong region?";
					StatusColor = Brushes.Red;
					break;
				case StatsStatus.Waiting:
					StatusText = "Not Currently in Battle - Stats are from Last Battle";
					StatusColor = Brushes.Black;
					break;
				default:
					StatusText = string.Empty;
					StatusColor = Brushes.Black;
					break;
			}
		}
	}
}

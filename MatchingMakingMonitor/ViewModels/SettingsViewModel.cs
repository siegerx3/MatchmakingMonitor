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

		private int fontSize;
		public int FontSize
		{
			get { return fontSize; }
			set
			{
				fontSize = value;
				if (initialized)
				{
					settings.FontSize = value;
					FirePropertyChanged();
				}
			}
		}

		private Color overall9;
		public Color Overall9
		{
			get { return overall9; }
			set
			{
				overall9 = value;
				if (initialized)
				{
					settings.Overall9 = value.ToString();
					FirePropertyChanged();
				}
			}
		}

		private Color overall8;
		public Color Overall8
		{
			get { return overall8; }
			set
			{
				overall8 = value;
				if (initialized)
				{
					settings.Overall8 = value.ToString();
					FirePropertyChanged();
				}
			}
		}

		private Color overall7;
		public Color Overall7
		{
			get { return overall7; }
			set
			{
				overall7 = value;
				if (initialized)
				{
					settings.Overall7 = value.ToString();
					FirePropertyChanged();
				}
			}
		}

		private Color overall6;
		public Color Overall6
		{
			get { return overall6; }
			set
			{
				overall6 = value;
				if (initialized)
				{
					settings.Overall6 = value.ToString();
					FirePropertyChanged();
				}
			}
		}

		private Color overall5;
		public Color Overall5
		{
			get { return overall5; }
			set
			{
				overall5 = value;
				if (initialized)
				{
					settings.Overall5 = value.ToString();
					FirePropertyChanged();
				}
			}
		}

		private Color overall4;
		public Color Overall4
		{
			get { return overall4; }
			set
			{
				overall4 = value;
				if (initialized)
				{
					settings.Overall4 = value.ToString();
					FirePropertyChanged();
				}
			}
		}

		private Color overall3;
		public Color Overall3
		{
			get { return overall3; }
			set
			{
				overall3 = value;
				if (initialized)
				{
					settings.Overall3 = value.ToString();
					FirePropertyChanged();
				}
			}
		}

		private Color overall2;
		public Color Overall2
		{
			get { return overall2; }
			set
			{
				overall2 = value;
				if (initialized)
				{
					settings.Overall2 = value.ToString();
					FirePropertyChanged();
				}
			}
		}

		private Color overall1;
		public Color Overall1
		{
			get { return overall1; }
			set
			{
				overall1 = value;
				if (initialized)
				{
					settings.Overall1 = value.ToString();
					FirePropertyChanged();
				}
			}
		}

		private LoggingService loggingService;
		private Services.Settings settings;
		private bool initialized;

		public SettingsWindowViewModel(LoggingService loggingService, Services.Settings settings)
		{
			this.loggingService = loggingService;
			this.settings = settings;

			this.ResetCommand = new RelayCommand(() =>
			{
				var result = MessageBox.Show("Are you sure you want to reset all settings?", "Reset settings", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes)
				{
					this.settings.ResetUI();
					initSettings();
				}
			});

			initSettings();
		}

		public SettingsWindowViewModel()
		{

		}

		private void initSettings()
		{
			try
			{
				FontSize = settings.FontSize;
				Overall9 = (Color)ColorConverter.ConvertFromString(settings.Overall9);
				Overall8 = (Color)ColorConverter.ConvertFromString(settings.Overall8);
				Overall7 = (Color)ColorConverter.ConvertFromString(settings.Overall7);
				Overall6 = (Color)ColorConverter.ConvertFromString(settings.Overall6);
				Overall5 = (Color)ColorConverter.ConvertFromString(settings.Overall5);
				Overall4 = (Color)ColorConverter.ConvertFromString(settings.Overall4);
				Overall3 = (Color)ColorConverter.ConvertFromString(settings.Overall3);
				Overall2 = (Color)ColorConverter.ConvertFromString(settings.Overall2);
				Overall1 = (Color)ColorConverter.ConvertFromString(settings.Overall1);
				initialized = true;
			}
			catch (Exception e)
			{
				loggingService.Error("Error while initializing settings", e);
			}
		}
	}
}

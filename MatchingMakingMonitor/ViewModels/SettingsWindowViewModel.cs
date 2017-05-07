using MatchMakingMonitor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MatchMakingMonitor.ViewModels
{
	public class SettingsWindowViewModel : BaseViewBinding
	{
		public RelayCommand ResetCommand { get; set; }

		public ObservableCollection<int> FontSizes { get; private set; } = new ObservableCollection<int>() { 8, 9, 10, 11, 12, 13, 14 };


		private string battleWeight;
		public string BattleWeight
		{
			get
			{
				if (battleWeight == null) battleWeight = Settings.BattleWeight.ToString();
				return battleWeight;
			}
			set
			{
				battleWeight = value;
				validateWeights();
				FirePropertyChanged();
			}
		}

		private string fragsWeight;
		public string FragsWeight
		{
			get
			{
				if (fragsWeight == null) fragsWeight = Settings.FragsWeight.ToString();
				return fragsWeight;
			}
			set
			{
				fragsWeight = value;
				validateWeights();
				FirePropertyChanged();
			}
		}

		private string xpWeight;
		public string XpWeight
		{
			get
			{
				if (xpWeight == null) xpWeight = Settings.XpWeight.ToString();
				return xpWeight;
			}
			set
			{
				xpWeight = value;
				validateWeights();
				FirePropertyChanged();
			}
		}

		private string dmgWeight;
		public string DmgWeight
		{
			get
			{
				if (dmgWeight == null) dmgWeight = Settings.DmgWeight.ToString();
				return dmgWeight;
			}
			set
			{
				dmgWeight = value;
				validateWeights();
				FirePropertyChanged();
			}
		}

		private string winWeight;
		public string WinWeight
		{
			get
			{
				if (winWeight == null) winWeight = Settings.WinWeight.ToString();
				return winWeight;
			}
			set
			{
				winWeight = value;
				validateWeights();
				FirePropertyChanged();
			}
		}

		private Visibility weightsErrorVisibility = Visibility.Collapsed;

		public Visibility WeightsErrorVisibility
		{
			get { return weightsErrorVisibility; }
			set
			{
				weightsErrorVisibility = value;
				FirePropertyChanged();
			}
		}

		private string weightsErrorTextTemplate = "Sum of weights needs to be 5. (Current: {0})";

		private string weightsErrorText;

		public string WeightsErrorText
		{
			get { return weightsErrorText; }
			set
			{
				weightsErrorText = value;
				FirePropertyChanged();
			}
		}


		private LoggingService loggingService;
		public Settings Settings { get; set; }
		public SettingsWindowViewModel(LoggingService loggingService, Services.Settings Settings)
		{
			this.loggingService = loggingService;
			this.Settings = Settings;

			this.ResetCommand = new RelayCommand(async () =>
			{
				var result = MessageBox.Show("Are you sure you want to reset all Settings?", "Reset Settings", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes)
				{
					winWeight = null;
					battleWeight = null;
					fragsWeight = null;
					xpWeight = null;
					dmgWeight = null;
					await this.Settings.ResetUI();
					FirePropertyChanged(nameof(WinWeight));
					FirePropertyChanged(nameof(BattleWeight));
					FirePropertyChanged(nameof(FragsWeight));
					FirePropertyChanged(nameof(XpWeight));
					FirePropertyChanged(nameof(DmgWeight));
				}
			});
		}

		public SettingsWindowViewModel()
		{

		}

		private bool validateWeights()
		{
			try
			{
				var winWeightD = double.Parse(WinWeight);
				var battleWeightD = double.Parse(BattleWeight);
				var fragsWeightD = double.Parse(FragsWeight);
				var xpWeightD = double.Parse(XpWeight);
				var dmgWeightD = double.Parse(DmgWeight);
				var sum = winWeightD + battleWeightD + fragsWeightD + xpWeightD + dmgWeightD;
				var valid = sum == 5;

				if (valid)
				{
					Settings.WinWeight = winWeightD;
					Settings.BattleWeight = battleWeightD;
					Settings.FragsWeight = fragsWeightD;
					Settings.XpWeight = xpWeightD;
					Settings.DmgWeight = dmgWeightD;
					WeightsErrorVisibility = Visibility.Collapsed;
				}
				else
				{
					WeightsErrorVisibility = Visibility.Visible;
					WeightsErrorText = string.Format(weightsErrorTextTemplate, sum);
				}
				return valid;
			}
			catch (Exception e)
			{
				WeightsErrorVisibility = Visibility.Visible;
				WeightsErrorText = "Invalid number";
				return false;
			}
		}
	}
}

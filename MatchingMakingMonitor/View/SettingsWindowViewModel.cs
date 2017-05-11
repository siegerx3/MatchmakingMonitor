using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using MatchMakingMonitor.Services;
using MatchMakingMonitor.View.Util;
using Microsoft.Win32;

namespace MatchMakingMonitor.View
{
	public class SettingsWindowViewModel : BaseViewBinding
	{
		public RelayCommand ResetCommand { get; set; }
		public RelayCommand ExportCommand { get; set; }
		public RelayCommand ImportCommand { get; set; }

		public ObservableCollection<int> FontSizes { get; } = new ObservableCollection<int> { 8, 9, 10, 11, 12, 13, 14 };


		private string _battleWeight;
		public string BattleWeight
		{
			get => _battleWeight ?? (_battleWeight = SettingsWrapper.CurrentSettings.BattleWeight.ToString(CultureInfo.InvariantCulture).Replace('.', ','));
			set
			{
				_battleWeight = value;
				ValidateWeights();
				FirePropertyChanged();
			}
		}

		private string _fragsWeight;
		public string FragsWeight
		{
			get => _fragsWeight ?? (_fragsWeight = SettingsWrapper.CurrentSettings.FragsWeight.ToString(CultureInfo.InvariantCulture).Replace('.', ','));
			set
			{
				_fragsWeight = value;
				ValidateWeights();
				FirePropertyChanged();
			}
		}

		private string _xpWeight;
		public string XpWeight
		{
			get => _xpWeight ?? (_xpWeight = SettingsWrapper.CurrentSettings.XpWeight.ToString(CultureInfo.InvariantCulture).Replace('.', ','));
			set
			{
				_xpWeight = value;
				ValidateWeights();
				FirePropertyChanged();
			}
		}

		private string _dmgWeight;
		public string DmgWeight
		{
			get => _dmgWeight ?? (_dmgWeight = SettingsWrapper.CurrentSettings.DmgWeight.ToString(CultureInfo.InvariantCulture).Replace('.', ','));
			set
			{
				_dmgWeight = value;
				ValidateWeights();
				FirePropertyChanged();
			}
		}

		private string _winWeight;
		public string WinWeight
		{
			get => _winWeight ?? (_winWeight = SettingsWrapper.CurrentSettings.WinWeight.ToString(CultureInfo.InvariantCulture).Replace('.', ','));
			set
			{
				_winWeight = value;
				ValidateWeights();
				FirePropertyChanged();
			}
		}

		private Visibility _weightsErrorVisibility = Visibility.Hidden;

		public Visibility WeightsErrorVisibility
		{
			get => _weightsErrorVisibility;
			set
			{
				_weightsErrorVisibility = value;
				FirePropertyChanged();
			}
		}

		private const string WeightsErrorTextTemplate = "Sum of weights needs to be 5. (Current: {0})";

		private string _weightsErrorText;

		public string WeightsErrorText
		{
			get => _weightsErrorText;
			set
			{
				_weightsErrorText = value;
				FirePropertyChanged();
			}
		}


		public SettingsWrapper SettingsWrapper { get; set; }

		[SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
		public SettingsWindowViewModel(SettingsWrapper settingsWrapper)
		{
			SettingsWrapper = settingsWrapper;

			ExportCommand = new RelayCommand(Export);
			ImportCommand = new RelayCommand(Import);

			ResetCommand = new RelayCommand(async () =>
			{
				var result = MessageBox.Show("Are you sure you want to reset all Settings?", "Reset Settings", MessageBoxButton.YesNo);
				if (result != MessageBoxResult.Yes) return;
				_winWeight = null;
				_battleWeight = null;
				_fragsWeight = null;
				_xpWeight = null;
				_dmgWeight = null;
				await Task.Run(() => SettingsWrapper.ResetUiSettings(settingsWrapper.CurrentSettings));
				FirePropertyChanged(nameof(WinWeight));
				FirePropertyChanged(nameof(BattleWeight));
				FirePropertyChanged(nameof(FragsWeight));
				FirePropertyChanged(nameof(XpWeight));
				FirePropertyChanged(nameof(DmgWeight));
			});
		}



		public SettingsWindowViewModel()
		{

		}

		private async void Import()
		{
			var ofd = new OpenFileDialog()
			{
				FileName = "settings.export.json",
				DefaultExt = ".json",
				Filter = "JSON Documents (.json)|*.json",
				CheckFileExists = true
			};
			if (ofd.ShowDialog() == true)
			{
				await SettingsWrapper.ImportUiSettings(ofd.FileName);
			}
		}

		private async void Export()
		{
			var sfd = new SaveFileDialog()
			{
				FileName = "settings.export",
				DefaultExt = ".json",
				Filter = "JSON Documents (.json)|*.json"
			};
			if (sfd.ShowDialog() == true)
			{
				await SettingsWrapper.ExportUiSettings(sfd.FileName);
			}
		}

		private void ValidateWeights()
		{
			try
			{
				var winWeightD = double.Parse(WinWeight);
				var battleWeightD = double.Parse(BattleWeight);
				var fragsWeightD = double.Parse(FragsWeight);
				var xpWeightD = double.Parse(XpWeight);
				var dmgWeightD = double.Parse(DmgWeight);
				var sum = Math.Round(winWeightD + battleWeightD + fragsWeightD + xpWeightD + dmgWeightD, 2);
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				var valid = sum == 5;

				if (valid)
				{
					SettingsWrapper.CurrentSettings.WinWeight = winWeightD;
					SettingsWrapper.CurrentSettings.BattleWeight = battleWeightD;
					SettingsWrapper.CurrentSettings.FragsWeight = fragsWeightD;
					SettingsWrapper.CurrentSettings.XpWeight = xpWeightD;
					SettingsWrapper.CurrentSettings.DmgWeight = dmgWeightD;
					WeightsErrorVisibility = Visibility.Hidden;
				}
				else
				{
					WeightsErrorVisibility = Visibility.Visible;
					WeightsErrorText = string.Format(WeightsErrorTextTemplate, sum);
				}
			}
			catch
			{
				WeightsErrorVisibility = Visibility.Visible;
				WeightsErrorText = "Invalid number";
			}
		}
	}
}

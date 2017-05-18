using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MatchMakingMonitor.View.Util;

namespace MatchMakingMonitor.View.Settings
{
	public class WeightsViewModel : ViewModelBase
	{
		private readonly WeightsEditor _weightsEditor;
		private bool suppressUpdate;
		private double actualSum;
		public WeightsViewModel(WeightsEditor weightsEditor)
		{
			_weightsEditor = weightsEditor;
			_weightsEditor.RegisterValuesChanged(UpdateValuesFromEditor);
			UpdateValuesFromEditor();
		}

		private void UpdateValuesFromEditor()
		{
			suppressUpdate = true;
			BattleWeight = _weightsEditor.BattleWeight.ToString(CultureInfo.InvariantCulture).Replace('.', ',');
			AvgFragsWeight = _weightsEditor.AvgFragsWeight.ToString(CultureInfo.InvariantCulture).Replace('.', ',');
			AvgXpWeight = _weightsEditor.AvgXpWeight.ToString(CultureInfo.InvariantCulture).Replace('.', ',');
			AvgDmgWeight = _weightsEditor.AvgDmgWeight.ToString(CultureInfo.InvariantCulture).Replace('.', ',');
			WinRateWeight = _weightsEditor.WinRateWeight.ToString(CultureInfo.InvariantCulture).Replace('.', ',');
			ValidationError = Visibility.Hidden;
			suppressUpdate = false;
		}

		public void LoadValues()
		{
			_weightsEditor.LoadValues();
		}

		private string _battleWeight;

		public string BattleWeight
		{
			get => _battleWeight;
			set
			{
				_battleWeight = value;
				if (!suppressUpdate)
					ValidationError = _weightsEditor.UpdateBattleWeight(_battleWeight, out actualSum) ? Visibility.Hidden : Visibility.Visible;
				FirePropertyChanged();
			}
		}

		private string _avgFragsWeight;

		public string AvgFragsWeight
		{
			get => _avgFragsWeight;
			set
			{
				_avgFragsWeight = value;
				if (!suppressUpdate)
					ValidationError = _weightsEditor.UpdateAvgFragsWeight(_avgFragsWeight, out actualSum) ? Visibility.Hidden : Visibility.Visible;
				FirePropertyChanged();
			}
		}

		private string _avgXpWeight;

		public string AvgXpWeight
		{
			get => _avgXpWeight;
			set
			{
				_avgXpWeight = value;
				if (!suppressUpdate)
					ValidationError = _weightsEditor.UpdateAvgXpWeight(_avgXpWeight, out actualSum) ? Visibility.Hidden : Visibility.Visible;
				FirePropertyChanged();
			}
		}

		private string _avgDmgWeight;

		public string AvgDmgWeight
		{
			get => _avgDmgWeight;
			set
			{
				_avgDmgWeight = value;
				if (!suppressUpdate)
					ValidationError = _weightsEditor.UpdateAvgDmgWeight(_avgDmgWeight, out actualSum) ? Visibility.Hidden : Visibility.Visible;
				FirePropertyChanged();
			}
		}

		private string _winRateWeight;

		public string WinRateWeight
		{
			get => _winRateWeight;
			set
			{
				_winRateWeight = value;
				if (!suppressUpdate)
					ValidationError = _weightsEditor.UpdateWinRateWeight(_winRateWeight, out actualSum) ? Visibility.Hidden : Visibility.Visible;
				FirePropertyChanged();
			}
		}

		private Visibility _validationError;
		public Visibility ValidationError
		{
			get => _validationError;
			set
			{
				_validationError = value;
				ValidationErrorText = $"Sum must be 5 but is {actualSum}";
				FirePropertyChanged();
			}
		}

		private string _validationErrorText;
		public string ValidationErrorText
		{
			get => _validationErrorText;
			set
			{
				_validationErrorText = value;
				FirePropertyChanged();
			}
		}
	}
}

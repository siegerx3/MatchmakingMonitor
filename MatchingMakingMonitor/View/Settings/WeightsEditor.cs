using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MatchMakingMonitor.config;
using MatchMakingMonitor.config.Reflection;

namespace MatchMakingMonitor.View.Settings
{
	public class WeightsEditor
	{
		private double _battleWeight;
		private double _avgFragsWeight;
		private double _avgXpWeight;
		private double _avgDmgWeight;
		private double _winRateWeight;

		private readonly BehaviorSubject<ChangedSetting> _changedSubject;
		private readonly SettingsJson _settings;
		private Action _valuesChanged;
		private readonly bool _initial;

		public WeightsEditor(BehaviorSubject<ChangedSetting> changedSubject, SettingsJson settings)
		{
			_changedSubject = changedSubject;
			_settings = settings;
			_initial = true;
			LoadValues();
			_initial = false;
		}

		public void RegisterValuesChanged(Action action)
		{
			_valuesChanged = action;
		}

		public void LoadValues()
		{
			_battleWeight = _settings.BattleWeight;
			_avgFragsWeight = _settings.AvgFragsWeight;
			_avgXpWeight = _settings.AvgXpWeight;
			_avgDmgWeight = _settings.AvgDmgWeight;
			_winRateWeight = _settings.WinRateWeight;
			_valuesChanged?.Invoke();
			if (!_initial)
			{
				_changedSubject.OnNext(new ChangedSetting(true, false));
			}
		}

		public double BattleWeight => _battleWeight;
		public double AvgFragsWeight => _avgFragsWeight;
		public double AvgXpWeight => _avgXpWeight;
		public double AvgDmgWeight => _avgDmgWeight;
		public double WinRateWeight => _winRateWeight;

		private static bool ValidateWeights(double winRateWeight, double battleWeight, double avgFragsWeight, double avgXpWeight, double avgDmgWeight, out double actualSum)
		{
			actualSum = 0;
			try
			{
				var sum = Math.Round(winRateWeight + battleWeight + avgFragsWeight + avgXpWeight + avgDmgWeight, 2);
				actualSum = sum;
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				return sum == 5;
			}
			catch
			{
				return false;
			}
		}

		public bool UpdateBattleWeight(string value, out double actualSum)
		{
			var oldValue = _battleWeight;
			double newValue;
			actualSum = 0;
			if (!double.TryParse(value, out newValue)) return false;
			if (!ValidateWeights(_winRateWeight, newValue, _avgFragsWeight, _avgXpWeight, _avgDmgWeight, out actualSum)) return false;
			_battleWeight = newValue;
			_settings.BattleWeight = newValue;
			_changedSubject.OnNext(new ChangedSetting(oldValue, _battleWeight));
			return true;
		}
		public bool UpdateAvgFragsWeight(string value, out double actualSum)
		{
			var oldValue = _avgFragsWeight;
			double newValue;
			actualSum = 0;
			if (!double.TryParse(value, out newValue)) return false;
			if (!ValidateWeights(_winRateWeight, _battleWeight, newValue, _avgXpWeight, _avgDmgWeight, out actualSum)) return false;
			_avgFragsWeight = newValue;
			_settings.AvgFragsWeight = newValue;
			_changedSubject.OnNext(new ChangedSetting(oldValue, _avgFragsWeight));
			return true;
		}
		public bool UpdateAvgXpWeight(string value, out double actualSum)
		{
			var oldValue = _avgXpWeight;
			double newValue;
			actualSum = 0;
			if (!double.TryParse(value, out newValue)) return false;
			if (!ValidateWeights(_winRateWeight, _battleWeight, _avgFragsWeight, newValue, _avgDmgWeight, out actualSum)) return false;
			_avgXpWeight = newValue;
			_settings.AvgXpWeight = newValue;
			_changedSubject.OnNext(new ChangedSetting(oldValue, _avgXpWeight));
			return true;
		}
		public bool UpdateAvgDmgWeight(string value, out double actualSum)
		{
			var oldValue = _avgDmgWeight;
			double newValue;
			actualSum = 0;
			if (!double.TryParse(value, out newValue)) return false;
			if (!ValidateWeights(_winRateWeight, _battleWeight, _avgFragsWeight, _avgXpWeight, newValue, out actualSum)) return false;
			_avgDmgWeight = newValue;
			_settings.AvgDmgWeight = newValue;
			_changedSubject.OnNext(new ChangedSetting(oldValue, _avgDmgWeight));
			return true;
		}
		public bool UpdateWinRateWeight(string value, out double actualSum)
		{
			var oldValue = _winRateWeight;
			double newValue;
			actualSum = 0;
			if (!double.TryParse(value, out newValue)) return false;
			if (!ValidateWeights(newValue, _battleWeight, _avgFragsWeight, _avgXpWeight, _avgDmgWeight, out actualSum)) return false;
			_winRateWeight = newValue;
			_settings.WinRateWeight = newValue;
			_changedSubject.OnNext(new ChangedSetting(oldValue, _winRateWeight));
			return true;
		}
	}
}

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
	public class WeightEditor
	{
		private double _battleWeight;
		private double _avgFragsWeight;
		private double _avgXpWeight;
		private double _avgDmgWeight;
		private double _winRateWeight;

		private readonly BehaviorSubject<ChangedSetting> _changedSubject;
		public WeightEditor(BehaviorSubject<ChangedSetting> changedSubject)
		{
			_changedSubject = changedSubject;
		}

		private bool UpdateValue(string value, ref double field, string key = null)
		{
			var oldValue = field;
			double newValue;
			if (!double.TryParse(value, out newValue)) return false;
			if (!ValidateWeights()) return false;
			field = newValue;
			_changedSubject.OnNext(new ChangedSetting(oldValue, field, key));
			return true;
		}

		private bool ValidateWeights()
		{
			try
			{
				var sum = Math.Round(_winRateWeight + _battleWeight + _avgFragsWeight + _avgXpWeight + _avgDmgWeight, 2);
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				return sum == 5;
			}
			catch
			{
				return false;
			}
		}

		public bool UpdateBattleWeight(string value)
		{
			return UpdateValue(value, ref _battleWeight, nameof(SettingsJson.BattleWeight));
		}
		public bool UpdateAvgFragsWeight(string value)
		{
			return UpdateValue(value, ref _avgFragsWeight, nameof(SettingsJson.AvgFragsWeight));
		}
		public bool UpdateAvgXpWeight(string value)
		{
			return UpdateValue(value, ref _avgXpWeight, nameof(SettingsJson.AvgXpWeight));
		}
		public bool UpdateAvgDmgWeight(string value)
		{
			return UpdateValue(value, ref _avgDmgWeight, nameof(SettingsJson.AvgDmgWeight));
		}
		public bool UpdateWinRateWeight(string value)
		{
			return UpdateValue(value, ref _winRateWeight, nameof(SettingsJson.WinRateWeight));
		}
	}
}

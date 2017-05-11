using MatchMakingMonitor.config.Reflection;
using Newtonsoft.Json;

namespace MatchMakingMonitor.config
{
	public class LimitValue : NotifySettingPropertyChanged
	{
		private double _value;

		[JsonProperty("value"), UiSetting]
		public double Value
		{
			get => _value;
			set
			{
				var oldValue = _value;
				_value = value;
				FirePropertyChanged(oldValue, value);
			}
		}
	}
}

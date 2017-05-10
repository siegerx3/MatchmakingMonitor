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
			set { _value = value; FirePropertyChanged(); }
		}
	}
}

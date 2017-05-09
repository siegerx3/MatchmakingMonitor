using MatchMakingMonitor.config.Reflection;
using Newtonsoft.Json;

namespace MatchMakingMonitor.config
{
	public class LimitValue : NotifySettingPropertyChanged
	{
		private int _value;

		[JsonProperty("value"), UiSetting]
		public int Value
		{
			get => _value;
			set { _value = value; FirePropertyChanged(); }
		}
	}
}

using MatchMakingMonitor.config.Reflection;
using Newtonsoft.Json;

namespace MatchMakingMonitor.config
{
	public class ColorValue
	{
		[JsonProperty("value"), UiSetting]
		public string Value { get; set; }
	}
}

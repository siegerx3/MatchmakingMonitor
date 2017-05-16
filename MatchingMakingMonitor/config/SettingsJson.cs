using MatchMakingMonitor.config.Reflection;
using Newtonsoft.Json;

namespace MatchMakingMonitor.config
{
	public class SettingsJson
	{

		[JsonProperty("installDirectory")]
		public string InstallDirectory { get; set; }

		[JsonProperty("region")]
		public Region Region { get; set; }

		[JsonProperty("appIds")]
		public AppId[] AppIds { get; set; }

		[JsonProperty("baseUrls")]
		public BaseUrl[] BaseUrls { get; set; }

		[JsonProperty("token")]
		public string Token { get; set; }

		[JsonProperty("colors"), NestedSetting]
		public ColorValue[] Colors { get; set; }

		[JsonProperty("fontSize"), UiSetting]
		public int FontSize { get; set; }

		[JsonProperty("battleLimits"), NestedSetting]
		public LimitValue[] BattleLimits { get; set; }

		[JsonProperty("winRateLimits"), NestedSetting]
		public LimitValue[] WinRateLimits { get; set; }

		[JsonProperty("avgFragsLimits"), NestedSetting]
		public LimitValue[] AvgFragsLimits { get; set; }

		[JsonProperty("avgXpLimits"), NestedSetting]
		public LimitsTier[] AvgXpLimits { get; set; }

		[JsonProperty("avgDmgLimits"), NestedSetting]
		public LimitsType AvgDmgLimits { get; set; }

		[JsonProperty("battleWeight"), UiSetting]
		public double BattleWeight { get; set; }

		[JsonProperty("avgFragsWeight"), UiSetting]
		public double AvgFragsWeight { get; set; }

		[JsonProperty("avgXpWeight"), UiSetting]
		public double AvgXpWeight { get; set; }

		[JsonProperty("avgDmgWeight"), UiSetting]
		public double AvgDmgWeight { get; set; }

		[JsonProperty("winRateWeight"), UiSetting]
		public double WinRateWeight { get; set; }
	}
}

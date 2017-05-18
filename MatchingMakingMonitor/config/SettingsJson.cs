using MatchMakingMonitor.config.Reflection;
using Newtonsoft.Json;

namespace MatchMakingMonitor.config
{
	public class SettingsJson
	{

		[JsonProperty("installDirectory")]
		public string InstallDirectory;

		[JsonProperty("region")]
		public Region Region;

		[JsonProperty("appIds")]
		public AppId[] AppIds;

		[JsonProperty("baseUrls")]
		public BaseUrl[] BaseUrls;

		[JsonProperty("token")]
		public string Token;

		[JsonProperty("colors"), ExportSetting]
		public string[] Colors;

		[JsonProperty("fontSize"), ExportSetting]
		public int FontSize;

		[JsonProperty("battleLimits"), ExportSetting]
		public double[] BattleLimits;
		[JsonProperty("winRateLimits"), ExportSetting]
		public double[] WinRateLimits;

		[JsonProperty("avgFragsLimits"), ExportSetting]
		public double[] AvgFragsLimits;

		[JsonProperty("avgXpLimits"), ExportSetting]
		public LimitsTier[] AvgXpLimits;

		[JsonProperty("avgDmgLimits"), ExportSetting]
		public LimitsType AvgDmgLimits;

		[JsonProperty("battleWeight"), ExportSetting]
		public double BattleWeight;

		[JsonProperty("avgFragsWeight"), ExportSetting]
		public double AvgFragsWeight;

		[JsonProperty("avgXpWeight"), ExportSetting]
		public double AvgXpWeight;

		[JsonProperty("avgDmgWeight"), ExportSetting]
		public double AvgDmgWeight;

		[JsonProperty("winRateWeight"), ExportSetting]
		public double WinRateWeight;
	}
}

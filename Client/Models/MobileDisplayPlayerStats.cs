using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MatchMakingMonitor.Models
{
	public class MobileDisplayPlayerStats
	{
		[JsonProperty("relation")] public int Relation { get; set; }
		[JsonProperty("privateOrHidden")] public bool PrivateOrHidden { get; set; }
		[JsonProperty("isLowBattles")] public bool IsLowBattles { get; set; }
		[JsonProperty("displayName")] public string DisplayName { get; set; }
		[JsonProperty("name")] public string Name { get; set; }
		[JsonProperty("accountId")] public string AccountId { get; set; }
		[JsonProperty("shipName")] public string ShipName { get; set; }
		[JsonProperty("battles")] public double Battles { get; set; }
		[JsonProperty("wins")] public double Wins { get; set; }
		[JsonProperty("winRate")] public double WinRate { get; set; }
		[JsonProperty("avgXp")] public double AvgXp { get; set; }
		[JsonProperty("avgFrags")] public double AvgFrags { get; set; }
		[JsonProperty("avgDamage")] public double AvgDamage { get; set; }

		[JsonProperty("colorNameKey")] public int ColorNameKey { get; set; }
		[JsonProperty("colorWinRateKey")] public int ColorWinRateKey { get; set; }
		[JsonProperty("colorAvgFragsKey")] public int ColorAvgFragsKey { get; set; }
		[JsonProperty("colorAvgXpKey")] public int ColorAvgXpKey { get; set; }
		[JsonProperty("colorAvgDamageKey")] public int ColorAvgDamageKey { get; set; }
		[JsonProperty("colorBattlesKey")] public int ColorBattlesKey { get; set; }
	}
}
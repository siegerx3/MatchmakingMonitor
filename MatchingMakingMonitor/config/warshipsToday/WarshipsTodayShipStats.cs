using Newtonsoft.Json;

namespace MatchMakingMonitor.config.warshipsToday
{
	public class WarshipsTodayShipStats
	{
		[JsonProperty(PropertyName = "damage_dealt")]
		public double DamageDealt;
		[JsonProperty(PropertyName = "xp")]
		public double Xp;
		[JsonProperty(PropertyName = "frags")]
		public double Frags;
		[JsonProperty(PropertyName = "battles")]
		public double Battles;
		[JsonProperty(PropertyName = "wins")]
		public double Wins;
	}
}
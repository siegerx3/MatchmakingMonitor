using Newtonsoft.Json;

namespace MatchMakingMonitor.Models.ResponseTypes
{
	public class WgShip
	{
		[JsonProperty("ship_id")]
		public long ShipId { get; set; }
		[JsonProperty("name")]
		public string Name { get; set; }
		[JsonProperty("type")]
		public string Type { get; set; }
		[JsonProperty("tier")]
		public int Tier { get; set; }
	}
}

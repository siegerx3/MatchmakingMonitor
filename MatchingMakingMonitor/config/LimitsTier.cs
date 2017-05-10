using MatchMakingMonitor.config.Reflection;
using MatchMakingMonitor.Models.ResponseTypes;
using Newtonsoft.Json;

namespace MatchMakingMonitor.config
{
	public class LimitsTier
	{
		[JsonProperty("tier")]
		public ShipTier ShipTier { get; set; }

		[JsonProperty("values"), NestedSetting]
		public LimitValue[] Values { get; set; }
	}
}

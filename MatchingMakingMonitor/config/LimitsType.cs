using System.Linq;
using MatchMakingMonitor.config.Reflection;
using MatchMakingMonitor.Models.ResponseTypes;
using Newtonsoft.Json;

namespace MatchMakingMonitor.config
{
	public class LimitsType : NestedSetting
	{
		[JsonProperty("destroyer"), NestedSetting]
		public LimitsTier[] Destroyer { get; set; }

		[JsonProperty("battleship"), NestedSetting]
		public LimitsTier[] Battleship { get; set; }

		[JsonProperty("cruiser"), NestedSetting]
		public LimitsTier[] Cruiser { get; set; }

		[JsonProperty("airCarrier"), NestedSetting]
		public LimitsTier[] AirCarrier { get; set; }

		public LimitValue[] GetLimits(ShipType shipType, ShipTier shipTier)
		{
			LimitsTier[] limitsTier;
			switch (shipType)
			{
				case ShipType.AirCarrier:
					limitsTier = AirCarrier;
					break;
				case ShipType.Battleship:
					limitsTier = Battleship;
					break;
				case ShipType.Destroyer:
					limitsTier = Destroyer;
					break;
				case ShipType.Cruiser:
					limitsTier = Destroyer;
					break;
				default:
					limitsTier = new LimitsTier[0];
					break;
			}
			return limitsTier.SingleOrDefault(l => l.ShipTier == shipTier)?.Values;
		}
	}
}

using System.Linq;
using MatchMakingMonitor.config.Reflection;
using MatchMakingMonitor.Models.ResponseTypes;
using Newtonsoft.Json;

namespace MatchMakingMonitor.config
{
	public class LimitsType
	{
		[JsonProperty("destroyer")]
		public LimitsTier[] Destroyer;

		[JsonProperty("battleship")]
		public LimitsTier[] Battleship;

		[JsonProperty("cruiser")]
		public LimitsTier[] Cruiser;

		[JsonProperty("airCarrier")]
		public LimitsTier[] AirCarrier;

		public double[] GetLimits(ShipType shipType, ShipTier shipTier)
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

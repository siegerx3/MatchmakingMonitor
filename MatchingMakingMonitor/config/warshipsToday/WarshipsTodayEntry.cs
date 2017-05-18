using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MatchMakingMonitor.config.warshipsToday
{
	public class WarshipsTodayEntry
	{
		[JsonProperty(PropertyName = "vehicle")]
		public WarshipsTodayVehicle Vehicle { get; set; }
		[JsonProperty(PropertyName = "statistics")]
		public WarshipsTodayStats Statistics { get; set; }

	}
}

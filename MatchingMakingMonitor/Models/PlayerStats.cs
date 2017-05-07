using Newtonsoft.Json;
using System.Collections.Generic;

namespace MatchMakingMonitor.Models
{
	public class Meta
	{
		public int count { get; set; }
		public object hidden { get; set; }
	}

	public class MainBattery
	{
		public int max_frags_battle { get; set; }
		public int frags { get; set; }
		public int hits { get; set; }
		public int shots { get; set; }
	}

	public class SecondBattery
	{
		public int max_frags_battle { get; set; }
		public int frags { get; set; }
		public int hits { get; set; }
		public int shots { get; set; }
	}

	public class Ramming
	{
		public int max_frags_battle { get; set; }
		public int frags { get; set; }
	}

	public class Torpedoes
	{
		public int max_frags_battle { get; set; }
		public int frags { get; set; }
		public int hits { get; set; }
		public int shots { get; set; }
	}

	public class Aircraft
	{
		public int max_frags_battle { get; set; }
		public int frags { get; set; }
	}

	public class Pvp
	{
		public int max_xp { get; set; }
		public int damage_to_buildings { get; set; }
		public MainBattery main_battery { get; set; }
		public int suppressions_count { get; set; }
		public int max_damage_scouting { get; set; }
		public int art_agro { get; set; }
		public int ships_spotted { get; set; }
		public SecondBattery second_battery { get; set; }
		public int xp { get; set; }
		public int survived_battles { get; set; }
		public int dropped_capture_points { get; set; }
		public int max_damage_dealt_to_buildings { get; set; }
		public int torpedo_agro { get; set; }
		public int draws { get; set; }
		public int planes_killed { get; set; }
		public int battles { get; set; }
		public int max_ships_spotted { get; set; }
		public int team_capture_points { get; set; }
		public int frags { get; set; }
		public int damage_scouting { get; set; }
		public int max_total_agro { get; set; }
		public int max_frags_battle { get; set; }
		public int capture_points { get; set; }
		public Ramming ramming { get; set; }
		public Torpedoes torpedoes { get; set; }
		public Aircraft aircraft { get; set; }
		public int survived_wins { get; set; }
		public int max_damage_dealt { get; set; }
		public int wins { get; set; }
		public int losses { get; set; }
		public int damage_dealt { get; set; }
		public int max_planes_killed { get; set; }
		public int max_suppressions_count { get; set; }
		public int team_dropped_capture_points { get; set; }
		public int battles_since_512 { get; set; }
	}

	public class Ship
	{
		public int distance { get; set; }
		public int last_battle_time { get; set; }
		public long account_id { get; set; }
		public Pvp pvp { get; set; }
		public int updated_at { get; set; }
		public int battles { get; set; }
		public long ship_id { get; set; }
		[JsonProperty(PropertyName = "private")]
		public string @private { get; set; }
	}

	public class Data
	{
		public List<Ship> Ships { get; set; }
	}

	public class PlayerStats
	{
		public string status { get; set; }
		public Meta meta { get; set; }
		public Data data { get; set; }
	}
}

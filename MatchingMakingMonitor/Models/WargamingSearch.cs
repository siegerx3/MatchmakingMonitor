using System.Collections.Generic;

namespace MatchingMakingMonitor.Models
{
    class WargamingSearch
    {
        public string status { get; set; }
        public Dictionary<string, int> meta { get; set; }
        public List<WargamingPlayers> data { get; set; }
    }

    class WargamingPlayers
    {
        public string nickname { get; set; }
        public long account_id { get; set; }
        public string Region { get; set; }
    } //end class
} //end namespace

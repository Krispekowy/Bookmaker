using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bukmacherka.Database.DTO
{
    public class Match
    {
        public string id { get; set; }
        public string date { get; set; }
        public string home_name { get; set; }
        public string away_name { get; set; }
        public string score { get; set; }
        public string ht_score { get; set; }
        public string ft_score { get; set; }
        public string et_score { get; set; }
        public string time { get; set; }
        public string league_id { get; set; }
        public string status { get; set; }
        public string added { get; set; }
        public string last_changed { get; set; }
        public string home_id { get; set; }
        public string away_id { get; set; }
        public string competition_id { get; set; }
        public string competition_name { get; set; }
        public string location { get; set; }
        public int fixture_id { get; set; }
        public string scheduled { get; set; }
    }

    public class DataMatches
    {
        public List<Match> match { get; set; }
        public string next_page { get; set; }
        public bool prev_page { get; set; }
    }

    public class MatchesDTO
    {
        public bool success { get; set; }
        public DataMatches data { get; set; }
    }


}

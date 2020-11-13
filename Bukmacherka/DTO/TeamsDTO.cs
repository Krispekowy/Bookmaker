using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bukmacherka.Database.DTO
{
    public class CountryTeams
    {
        public string id { get; set; }
        public string name { get; set; }
        public string is_real { get; set; }
    }

    public class Team
    {
        public string id { get; set; }
        public string name { get; set; }
        public string stadium { get; set; }
        public CountryTeams country { get; set; }
        public List<string> federation { get; set; }
        public object name_ru { get; set; }
        public object translations { get; set; }
    }

    public class DataTeams
    {
        public List<Team> teams { get; set; }
        public string total { get; set; }
        public int pages { get; set; }
        public string next_page { get; set; }
        public string prev_page { get; set; }
    }

    public class TeamsDTO
    {
        public bool success { get; set; }
        public DataTeams data { get; set; }
    }
}

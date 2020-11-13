using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bukmacherka.Database.Entities
{
    public class CountriesDTO
    {
            public bool success { get; set; }
            public Data data { get; set; }
        
    }
    public class Data
    {
        public List<Country> country { get; set; }
    }
    public class NationalTeam
    {
        public string id { get; set; }
        public string name { get; set; }
        public string stadium { get; set; }
        public string location { get; set; }
    }

    public class Federation
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Country
    {
        public string id { get; set; }
        public string name { get; set; }
        public string is_real { get; set; }
        public string leagues { get; set; }
        public string scores { get; set; }
        public NationalTeam national_team { get; set; }
        public Federation federation { get; set; }
    }
}

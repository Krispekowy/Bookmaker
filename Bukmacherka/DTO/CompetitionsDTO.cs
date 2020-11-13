using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bukmacherka.Database.DTO
{
    public class Country
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Federation
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Competition
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<Country> countries { get; set; }
        public List<Federation> federations { get; set; }
    }

    public class Data
    {
        public List<Competition> competition { get; set; }
    }

    public class CompetitionsDTO
    {
        public bool success { get; set; }
        public Data data { get; set; }
    }
}

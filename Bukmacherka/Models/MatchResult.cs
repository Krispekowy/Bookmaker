using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bukmacherka.ViewModels
{
    public class MatchResult
    {
        public string Date { get; set; }
        public string Score { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string IsDraw { get; set; }
    }
}

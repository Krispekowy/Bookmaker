using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bukmacherka.ViewModels
{
    public class TeamCheckResult
    {
        public List<MatchResult> MatchResults { get; set; }
        public int NoDrawSeries { get; set; }
    }
}

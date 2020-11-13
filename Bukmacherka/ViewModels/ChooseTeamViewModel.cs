using Bukmacherka.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bukmacherka.ViewModels
{
    public class ChooseTeamViewModel
    {
        [Key]
        public string CountryId { get; set; }
        [Display(Name = "Kraj")]
        public string CountryName { get; set; }
        [NotMapped]
        public int LeagueId { get; set; }
        [NotMapped]
        public int TeamId { get; set; }
        public TeamCheckResult TeamResult { get; set; }
        public IEnumerable<SeriesWithoutDraw> seriesWithoutDraws { get; set; }
    }
}

using Bukmacherka.Models;
using Bukmacherka.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bukmacherka.Interfaces
{
    public interface ILiveScoreAPIService
    {
        IEnumerable<SelectListItem> GetSelectListItems(IEnumerable<Dropdown> elements);
        string CheckResult(string model);
        Task<List<Dropdown>> GetCountries();
        Task<List<Dropdown>> GetCompetitons(string countryId);
        Task<List<Dropdown>> GetTeams(string countryId);
        Task<TeamCheckResult> CheckTeam(string competitionId, string teamId);
        Task<List<SeriesWithoutDraw>> CheckSeriesTeams(List<Dropdown> teams, string competitionId);
    }
}

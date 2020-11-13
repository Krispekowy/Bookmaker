using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bukmacherka.Interfaces;
using Bukmacherka.Models;
using Bukmacherka.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bukmacherka.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILiveScoreAPIService _liveScoreAPI;

        public HomeController(
            ILiveScoreAPIService liveScoreAPI)
        {
            _liveScoreAPI = liveScoreAPI;
        }
        public async Task<IActionResult> Index()
        {
            var countries = new List<Dropdown>();
            // Let's get all countries that we need for a DropDownList
            countries = await _liveScoreAPI.GetCountries();

            // Inserting select list item in list
            var empty = countries.Find(a => a.Key == "0");
            if (empty == null)
            {
                countries.Insert(0, new Dropdown { Key = "0", Value = "-- Wybierz kraj --" });
            }
            ViewBag.ListOfCountries = countries;
            var model = new ChooseTeamViewModel();
            model.TeamResult = new TeamCheckResult();
            model.TeamResult.MatchResults = new List<MatchResult>();
            return View(model);
        }
        public async Task<JsonResult> GetCompetitions(string countryId)
        {
            var teams = await _liveScoreAPI.GetCompetitons(countryId);

            teams.Insert(0, new Dropdown { Key = "0", Value = "-- Wybierz ligę/turniej --" });

            return Json(new SelectList(teams, "Key", "Value"));
        }

        public async Task<JsonResult> GetTeams(string countryId)
        {

            var teams = await _liveScoreAPI.GetTeams(countryId);

            teams.Insert(0, new Dropdown { Key = "0", Value = "-- Wybierz zespół --" });

            return Json(new SelectList(teams, "Key", "Value"));
        }

        public async Task<JsonResult> CheckTeam(string competitionId, string teamId)
        {
            var teams = await _liveScoreAPI.CheckTeam(competitionId,teamId);
            return Json(teams);
        }
        public async Task<JsonResult> CheckSeriesTeams(string competitionId, string countryId)
        {
            var teams = await _liveScoreAPI.GetTeams(countryId);
            var series = await _liveScoreAPI.CheckSeriesTeams(teams, competitionId);
            return Json(series);
        }
    }
}

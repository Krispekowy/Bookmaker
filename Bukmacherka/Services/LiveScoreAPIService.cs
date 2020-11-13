using Bukmacherka.Database.DTO;
using Bukmacherka.Database.Entities;
using Bukmacherka.Interfaces;
using Bukmacherka.Models;
using Bukmacherka.ViewModels;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bukmacherka.Services
{
    public class LiveScoreAPIService : ILiveScoreAPIService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public LiveScoreAPIService(IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        //Method checks if the result was a draw
        public string CheckResult(string result)
        {

            if (result.Substring(0, 1) == result.Substring(result.Length - 1, 1))
            {
                return "TAK";
            }
            else
            {
                return "NIE";
            }
        }

        //Method gets list of countries
        public async Task<List<Dropdown>> GetCountries()
        {
            var listCountries = new List<Dropdown>();
            
            if (!_memoryCache.TryGetValue("Countries", out listCountries))
            {
                var _httpClient = _httpClientFactory.CreateClient("myAPIClient");
                HttpResponseMessage responseMessage = await _httpClient.GetAsync(_httpClient.BaseAddress + "countries/list.json?&key=" + _configuration["AuthenticationAPI:LIVESCORE_API_KEY"] +"&secret="+ _configuration["AuthenticationAPI:LIVESCORE_API_SECRET"]);
                if (responseMessage.IsSuccessStatusCode)
                {
                    listCountries = new List<Dropdown>();
                    string response = await responseMessage.Content.ReadAsStringAsync();
                    var countries = JsonConvert.DeserializeObject<CountriesDTO>(response);
                    foreach (var country in countries.data.country)
                    {
                        if (country.is_real == "1")
                        {
                            var dropdownValue = new Dropdown()
                            {
                                Key = country.id,
                                Value = country.name
                            };
                            listCountries.Add(dropdownValue);
                        }
                    }
                    _memoryCache.Set("Countries", listCountries);
                    return listCountries;
                }
            }
            listCountries = _memoryCache.Get("Countries") as List<Dropdown>;
            return listCountries;
        }

        //Method create List<SelectListItem>
        public IEnumerable<SelectListItem> GetSelectListItems(IEnumerable<Dropdown> elements)
        {
            // Create an empty list to hold result of the operation
            var selectList = new List<SelectListItem>();

            // For each string in the 'elements' variable, create a new SelectListItem object
            // that has both its Value and Text properties set to a particular value.
            // This will result in MVC rendering each item as:
            // <option value="CountryId">CountryName</option>
            foreach (var element in elements)
            {
                selectList.Add(new SelectListItem
                {
                    Value = element.Key,
                    Text = element.Value
                });
            }

            return selectList;
        }

        //Method gets list of teams by country
        public async Task<List<Dropdown>> GetTeams(string countryId)
        {
            var listOfTeams = new List<Dropdown>();

            var _httpClient = _httpClientFactory.CreateClient("myAPIClient");
            HttpResponseMessage responseMessage = await _httpClient.GetAsync(_httpClient.BaseAddress + "teams/list.json?key=" + _configuration["AuthenticationAPI:LIVESCORE_API_KEY"] + "&secret=" + _configuration["AuthenticationAPI:LIVESCORE_API_SECRET"] +"&size=100&language=pl&country_id=" + countryId + "&page=1");
            if (responseMessage.IsSuccessStatusCode)
            {
                string response = await responseMessage.Content.ReadAsStringAsync();
                var teams = JsonConvert.DeserializeObject<TeamsDTO>(response);
                int page = 1;
                while (teams.data.pages >= page)
                {
                    page++;
                    responseMessage = await _httpClient.GetAsync(_httpClient.BaseAddress + "teams/list.json?key=" + _configuration["AuthenticationAPI:LIVESCORE_API_KEY"] + "&secret=" + _configuration["AuthenticationAPI:LIVESCORE_API_SECRET"]+"&size=100&language=pl&country_id=" + countryId + "&page=" + page);
                    response = await responseMessage.Content.ReadAsStringAsync();
                    var teamsNextPage = JsonConvert.DeserializeObject<TeamsDTO>(response);
                    teams.data.teams.AddRange(teamsNextPage.data.teams);
                }
                foreach (var team in teams.data.teams)
                {
                    var dropdownValue = new Dropdown()
                    {
                        Key = team.id,
                        Value = team.name
                    };
                    listOfTeams.Add(dropdownValue);
                }
                return listOfTeams;
            }
            return null;
        }

        //Method gets list of competitions by country
        public async Task<List<Dropdown>> GetCompetitons(string countryId)
        {
            var listOfCompetitions = new List<Dropdown>();
            if (!_memoryCache.TryGetValue("Competitions" + countryId, out listOfCompetitions))
            {
                listOfCompetitions = new List<Dropdown>();
                var _httpClient = _httpClientFactory.CreateClient("myAPIClient");
                HttpResponseMessage responseMessage = await _httpClient.GetAsync(_httpClient.BaseAddress + "competitions/list.json?key=" + _configuration["AuthenticationAPI:LIVESCORE_API_KEY"] + "&secret=" + _configuration["AuthenticationAPI:LIVESCORE_API_SECRET"]+"&country_id=" + countryId);
                if (responseMessage.IsSuccessStatusCode)
                {
                    string response = await responseMessage.Content.ReadAsStringAsync();
                    var competitions = JsonConvert.DeserializeObject<CompetitionsDTO>(response);
                    
                    foreach (var competition in competitions.data.competition)
                    {
                        var dropdownValue = new Dropdown()
                        {
                            Key = competition.id,
                            Value = competition.name
                        };
                        listOfCompetitions.Add(dropdownValue);
                    }
                    _memoryCache.Set("Competitions" + countryId, listOfCompetitions);
                    return listOfCompetitions;
                }
            }
            listOfCompetitions = _memoryCache.Get("Competitions" + countryId) as List<Dropdown>;
            return listOfCompetitions;
        }

        //Method checks last 30 results (or less) from 2020-01-01 for team by competition.
        public async Task<TeamCheckResult> CheckTeam(string competitionId, string teamId)
        {
            var model = new TeamCheckResult();
            //GET Method
            var _httpClient = _httpClientFactory.CreateClient("myAPIClient");
            HttpResponseMessage responseMessage = await _httpClient.GetAsync(_httpClient.BaseAddress + "scores/history.json?key=" + _configuration["AuthenticationAPI:LIVESCORE_API_KEY"] + "&secret=" + _configuration["AuthenticationAPI:LIVESCORE_API_SECRET"]+"&from=2020-01-01&team=" + teamId +"&competition_id=" +competitionId);
            if (responseMessage.IsSuccessStatusCode)
            {
                string response = await responseMessage.Content.ReadAsStringAsync();
                var matches = JsonConvert.DeserializeObject<MatchesDTO>(response);
                return CreateListOfLastMatches(model, matches);
            }
            return null;
        }

        //Method create model last matches
        private TeamCheckResult CreateListOfLastMatches(TeamCheckResult model, MatchesDTO matches)
        {
            var listOfResults = new List<MatchResult>();
            int seriesWithoutDraw = 0;
            foreach (var match in matches.data.match)
            {
                var matchResult = new MatchResult()
                {
                    Date = match.date,
                    HomeTeam = match.home_name,
                    Score = match.score,
                    AwayTeam = match.away_name
                };
                matchResult.IsDraw = CheckResult(matchResult.Score);
                listOfResults.Add(matchResult);
                if (matchResult.IsDraw == "NIE")
                {
                    seriesWithoutDraw++;
                }
                else
                {
                    seriesWithoutDraw = 0;
                }
                model.NoDrawSeries = seriesWithoutDraw;
            }
            model.MatchResults = listOfResults;
            return model;
        }

        //Method checks series of draw matches per competition
        public async Task<List<SeriesWithoutDraw>> CheckSeriesTeams(List<Dropdown> teams, string competitionId)
        {
            var series = new List<SeriesWithoutDraw>();
            if (!_memoryCache.TryGetValue("Series" + competitionId, out series))
            {
                var _httpClient = _httpClientFactory.CreateClient("myAPIClient");
                series = new List<SeriesWithoutDraw>();
                for (int i = 0; i < teams.Count; i++)
                {
                    HttpResponseMessage responseMessage = await _httpClient.GetAsync(_httpClient.BaseAddress + "scores/history.json?key=" + _configuration["AuthenticationAPI:LIVESCORE_API_KEY"] + "&secret=" + _configuration["AuthenticationAPI:LIVESCORE_API_SECRET"]+"&from=2020-01-01&team=" + teams[i].Key + "&competition_id=" + competitionId);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        string response = await responseMessage.Content.ReadAsStringAsync();
                        var matches = JsonConvert.DeserializeObject<MatchesDTO>(response);
                        
                        if (matches.data.match.Count == 0)
                        {
                            continue;
                        }
                        int seriesWithoutDraw = 0;

                        foreach (var match in matches.data.match)
                        {

                            var isDraw = CheckResult(match.score);
                            if (isDraw == "NIE")
                            {
                                seriesWithoutDraw++;
                            }
                            else
                            {
                                seriesWithoutDraw = 0;
                            }
                        }
                        var teamSeries = new SeriesWithoutDraw()
                        {
                            Series = seriesWithoutDraw,
                            TeamName = teams[i].Value,
                            LastMatch = matches.data.match[matches.data.match.Count - 1].home_name + " " + matches.data.match[matches.data.match.Count - 1].score + " " + matches.data.match[matches.data.match.Count - 1].away_name
                        };
                        series.Add(teamSeries);
                    }

                }
                int lp = series.Count;
                foreach (var item in series)
                {
                    item.Lp = lp;
                    lp = lp - 1;
                }
                _memoryCache.Set("Series" + competitionId, series);
                return series;
            }
            series = _memoryCache.Get("Series" + competitionId) as List<SeriesWithoutDraw>;
            return series;

        }

        //Unactive methods
        //public MatchesModel DeserializeJSON(string JSONresult)
        //{
        //    var matches = JsonConvert.DeserializeObject<MatchesModel>(JSONresult);
        //    return matches;
        //}

        //public async Task<string> GetJsonMatches(string dateFrom)
        //{
        //    try
        //    {
        //        //GET Method
        //        var _httpClient = _httpClientFactory.CreateClient("myAPIClient");
        //        HttpResponseMessage responseMessage = await _httpClient.GetAsync(_httpClient.BaseAddress + "scores/history.json?number=30&from=" + dateFrom + "&team=19" + "&key=" + _configuration.GetValue<string>("AuthenticationAPI:LIVESCORE_API_KEY") + "&secret=" + _configuration.GetValue<string>("AuthenticationAPI:LIVESCORE_API_SECRET"));
        //        if (responseMessage.IsSuccessStatusCode)
        //        {
        //            string response = await responseMessage.Content.ReadAsStringAsync();
        //            return response;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return "Something was wrong...:" + ex;
        //    }

        //    return null;
        //}
    }


}


using Microsoft.AspNetCore.Mvc;
using WikstromIT.Services;
using System.Globalization;
using WikstromIT.Configuration;
using Microsoft.Extensions.Options;

namespace WikstromIT.Controllers
{
    public class F1Controller : Controller
    {
        private readonly IFastF1Service _fastF1Service;
        private readonly FastF1ApiOptions _apiOptions;

        public F1Controller(IFastF1Service fastF1Service, IOptions<FastF1ApiOptions> apiOptions)
        {
            _fastF1Service = fastF1Service;
            _apiOptions = apiOptions.Value;
        }

        public IActionResult Index()
        {
            ViewData["FastF1BaseUrl"] = _apiOptions.BaseUrl;
            return PartialView();
        }

        public async Task<IActionResult> NextRace()
        {
            var raceInfo = await _fastF1Service.GetNextRaceAsync();

            if (raceInfo != null && !string.IsNullOrEmpty(raceInfo.NextRaceName) &&
                raceInfo.RaceDay.HasValue && raceInfo.RaceTime.HasValue)
            {
                var raceDateTime = raceInfo.RaceDay.Value.ToDateTime(raceInfo.RaceTime.Value);

                // Calculate time until race start
                var raceDateTimeUtc = DateTime.Parse($"{raceInfo.RaceDayUtc} {raceInfo.RaceTimeUtc}");
                var now = DateTime.UtcNow;
                var countdown = raceDateTime - now;

                raceInfo.CountdownDays = countdown.Days;
                raceInfo.CountdownHours = countdown.Hours;
                raceInfo.CountdownMinutes = countdown.Minutes;
                raceInfo.CountdownSeconds = countdown.Seconds;
            }

            return PartialView("_NextRace", raceInfo);
        }

        public async Task<IActionResult> TeamStandings()
        {
            var standings = await _fastF1Service.GetTeamStandingsAsync();
            return PartialView("_TeamStandings", standings);
        }

        public async Task<IActionResult> DriverStandings()
        {
            var standings = await _fastF1Service.GetDriverStandingsAsync();
            return PartialView("_DriverStandings", standings);
        }
    }
}

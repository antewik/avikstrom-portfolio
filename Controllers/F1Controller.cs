using Microsoft.AspNetCore.Mvc;
using AvikstromPortfolio.Configuration;
using Microsoft.Extensions.Options;
using AvikstromPortfolio.Services.F1;
using AvikstromPortfolio.Models.F1;

namespace AvikstromPortfolio.Controllers
{
    public class F1Controller : Controller
    {
        private readonly IF1InfoService _f1InfoService;
        private readonly F1InfoApiOptions _apiOptions;
        private readonly IF1NationalityMapper _mapper;
        private readonly ILogger<F1Controller> _logger;

        public F1Controller(
            IF1InfoService f1InfoService,
            IOptions<F1InfoApiOptions> apiOptions,
            IF1NationalityMapper mapper,
            ILogger<F1Controller> logger)
        {
            _f1InfoService = f1InfoService;
            _apiOptions = apiOptions.Value;
            _mapper = mapper;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("F1 Index page loaded");
            ViewData["F1InfoBaseUrl"] = _apiOptions.BaseUrl;

            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                // For AJAX navigation: return only the partial view
                return PartialView();
            }

            // For direct navigation/refresh: return full view
            return View();
        }

        public async Task<IActionResult> NextRace()
        {
            _logger.LogInformation("NextRace endpoint called");

            var raceInfo = await _f1InfoService.GetNextRaceAsync();

            if (raceInfo == null)
            {
                _logger.LogWarning("NextRace: No race info returned from F1InfoService");
                return PartialView("_NextRace", new RaceInfo { Message = "No upcoming races found." });
            }

            if (raceInfo.Sessions == null || !raceInfo.Sessions.Any())
            {
                _logger.LogWarning("NextRace: No race info returned from F1InfoService");
                return PartialView("_NextRace", raceInfo);
            }

            var raceSession = raceInfo.Sessions.FirstOrDefault(s => s.Label.Equals("Race", StringComparison.OrdinalIgnoreCase));

            if (raceSession?.UtcDateTime != null)
            {
                var countdown = raceSession.UtcDateTime.Value - DateTime.UtcNow;
                raceInfo.CountdownDays = countdown.Days;
                raceInfo.CountdownHours = countdown.Hours;
                raceInfo.CountdownMinutes = countdown.Minutes;
                raceInfo.CountdownSeconds = countdown.Seconds;
            }

            return PartialView("_NextRace", raceInfo);
        }

        public async Task<IActionResult> TeamStandings()
        {
            _logger.LogInformation("TeamStandings endpoint called");

            var standings = await _f1InfoService.GetTeamStandingsAsync();

            if (standings == null)
            {
                _logger.LogWarning("TeamStandings: No team' standings returned from F1InfoService");
                return PartialView("_TeamStandings", null);
            }

            foreach (var team in standings)
            {
                var country = _mapper.GetCountryInfo(team.Nationality);

                if (country == null)
                {
                    _logger.LogWarning("Nationality mapping failed for: {Nationality}", team.Nationality);
                }

                team.CountryCode = country?.IsoAlpha3 ?? "-";
            }

            return PartialView("_TeamStandings", standings);
        }

        public async Task<IActionResult> DriverStandings()
        {
            _logger.LogInformation("DriverStandings endpoint called");

            var standings = await _f1InfoService.GetDriverStandingsAsync();

            if (standings == null)
            {
                _logger.LogWarning("DriverStandings: No drivers' standings returned from F1InfoService");
                return PartialView("_DriverStandings", null);
            }

            foreach (var driver in standings.DriverStandings)
            {
                var country = _mapper.GetCountryInfo(driver.Nationality);

                if (country == null)
                {
                    _logger.LogWarning("Nationality mapping failed for: {Nationality}", driver.Nationality);
                }

                driver.CountryCode = country?.IsoAlpha3 ?? "-";
            }

            return PartialView("_DriverStandings", standings);
        }
    }
}

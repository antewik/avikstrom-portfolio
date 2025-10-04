using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using AvikstromPortfolio.Configuration;
using AvikstromPortfolio.Helpers;
using AvikstromPortfolio.Hubs;
using AvikstromPortfolio.Models.FlightInfo;
using AvikstromPortfolio.Services.FlightBoard;

namespace AvikstromPortfolio.Controllers
{
    /// <summary>
    /// MVC controller for the real-time flight board.
    /// Handles initial page load, caching of flight data, and integration with SignalR hub updates.
    /// </summary>
    public class FlightBoardController : Controller
    {
        private readonly IFlightInfoService _flightInfoService;
        private readonly FlightBoardPollingOptions _optionsPolling;
        private readonly FlightInfoApiOptions _optionsApi;
        private readonly ExternalScriptsOptions _optionScripts;
        private readonly ILogger<FlightBoardController> _logger;

        public FlightBoardController(
            IFlightInfoService flightInfoService,
            IHubContext<FlightBoardHub> hubContext,
            IOptions<FlightBoardPollingOptions> optionsPolling,
            IOptions<FlightInfoApiOptions> optionsApi,
            IOptions<ExternalScriptsOptions> optionsScripts,
            ILogger<FlightBoardController> logger)
        {
            _flightInfoService = flightInfoService;
            _optionsPolling = optionsPolling.Value;
            _optionsApi = optionsApi.Value;
            _optionScripts = optionsScripts.Value;
            _logger = logger;
        }

        /// <summary>
        /// Main entry point for the flight board view.
        /// Uses cached flight data when available, otherwise fetches from the API.
        /// Supports full view rendering, partial updates, and AJAX navigation.
        /// </summary>
        public async Task<IActionResult> Index(string iataCode, string direction, bool partial = false)
        {
            _logger.LogInformation("FlightBoard page loaded");

            List<FlightInfo> flights = [];

            // Only fetch data if form has been submitted
            if (!string.IsNullOrWhiteSpace(iataCode) && !string.IsNullOrWhiteSpace(direction))
            {
                var groupKey = $"{iataCode}:{direction}";

                // Try to use cache first
                if (FlightBoardPollingActivator.TryGetCache(groupKey, out var cache) && cache.Initialized)
                {
                    flights = cache.DisplayedFlights;
                    _logger.LogInformation("Index used cached flights for {GroupKey}", groupKey);
                }
                else
                {
                    // Seed cache with API call
                    flights = await FetchFlightsAsync(iataCode, direction);

                    var newCache = new FlightBoardCacheHelper();
                    newCache.Initialize(flights ?? new List<FlightInfo>(),
                                        displayCount: _optionsPolling.DisplayCount,
                                        direction,
                                        iataCode);

                    FlightBoardPollingActivator.SetCache(groupKey, newCache);

                    flights = newCache.DisplayedFlights;

                    if (flights.Count == 0)
                        _logger.LogWarning("No flights returned for {IataCode} {Direction}", iataCode, direction);
                    else
                        _logger.LogInformation("Index seeded cache for {GroupKey}", groupKey);
                }
            }

            ViewData["SearchPerformed"] = !string.IsNullOrWhiteSpace(iataCode) && !string.IsNullOrWhiteSpace(direction);
            ViewData["SignalRUrl"] = _optionScripts.SignalR;
            ViewBag.ApiRefreshInterval = GetFormattedTimeString();


            if (partial)
            {
                return PartialView("_FlightBoardResults", flights);
            }

            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                // For AJAX navigation: return only the partial view
                return PartialView("Index", flights);
            }

            // For direct navigation/refresh: return full view
            return View("Index", flights);
        }

        /// <summary>
        /// Fetches flight data from the API based on airport code and direction.
        /// Returns departures or arrivals depending on the specified direction.
        /// </summary>
        private async Task<List<FlightInfo>> FetchFlightsAsync(string iataCode, string direction)
        {
            try
            {
                if (direction == "Departure")
                {
                    return await _flightInfoService.GetDeparturesAsync(iataCode, _optionsApi.OffsetMinutes, _optionsApi.DurationMinutes);
                }
                else
                {
                    return await _flightInfoService.GetArrivalsAsync(iataCode, _optionsApi.OffsetMinutes, _optionsApi.DurationMinutes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch flights for {IataCode} {Direction}", iataCode, direction);
                return new List<FlightInfo>();
            }
        }

        private string GetFormattedTimeString()
        {
            var seconds = _optionsPolling.ApiRefreshSeconds;
            var minutes = seconds / 60;
            var remainder = seconds % 60;

            string refreshText;
            if (minutes > 0)
            {
                refreshText = $"{minutes} minute{(minutes > 1 ? "s" : "")}";
                if (remainder > 0)
                    refreshText += $" {remainder} second{(remainder > 1 ? "s" : "")}";
            }
            else
            {
                refreshText = $"{seconds} second{(seconds > 1 ? "s" : "")}";
            }

            return refreshText;
        }
    }
}

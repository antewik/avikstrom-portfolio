using Microsoft.Extensions.Options;
using AvikstromPortfolio.Configuration;
using AvikstromPortfolio.Hubs;
using System.Collections.Concurrent;
using AvikstromPortfolio.Helpers;

namespace AvikstromPortfolio.Services.FlightBoard
{
    /// <summary>
    /// Activates and manages background polling for flight board updates.
    /// Maintains per-group caches, rotates displayed flights, refreshes API data,
    /// and broadcasts updates to connected clients.
    /// </summary>
    public class FlightBoardPollingActivator : IFlightBoardPollingActivator
    {
        private readonly IServiceProvider _services;
        private CancellationTokenSource? _cts;
        private readonly FlightBoardPollingOptions _optionsPolling;
        private readonly FlightInfoApiOptions _optionsApi;
        private readonly ILogger<FlightBoardPollingActivator> _logger;

        // Cache per group: groupKey -> FlightCache
        private static readonly ConcurrentDictionary<string, FlightBoardCacheHelper> _flightCaches = new();

        public FlightBoardPollingActivator(
            IServiceProvider services,
            IOptions<FlightBoardPollingOptions> optionsPolling,
            IOptions<FlightInfoApiOptions> optionsApi,
            ILogger<FlightBoardPollingActivator> logger)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _optionsPolling = optionsPolling.Value ?? throw new ArgumentNullException(nameof(optionsPolling));
            _optionsApi = optionsApi.Value ?? throw new ArgumentNullException(nameof(optionsApi));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Starts the polling loop if not already running.
        /// </summary>
        public void TryStartPolling()
        {
            if (_cts != null) return; // already running
            _cts = new CancellationTokenSource();
            Task.Run(() => PollLoop(_cts.Token));
        }

        /// <summary>
        /// Stops the polling loop and clears all cached flight data.
        /// </summary>
        public void StopPolling()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            _flightCaches.Clear();
        }

        /// <summary>
        /// Attempts to retrieve the cache for a given group key.
        /// </summary>
        public static bool TryGetCache(string groupKey, out FlightBoardCacheHelper cache)
        {
            return _flightCaches.TryGetValue(groupKey, out cache);
        }

        /// <summary>
        /// Sets or replaces the cache for a given group key.
        /// </summary>
        public static void SetCache(string groupKey, FlightBoardCacheHelper cache)
        {
            _flightCaches[groupKey] = cache;
        }

        // <summary>
        /// Background loop that rotates cached flights, refreshes API data at intervals,
        /// and broadcasts updates to clients via SignalR.
        /// </summary>
        private async Task PollLoop(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    using var scope = _services.CreateScope();
                    var broadcaster = scope.ServiceProvider.GetRequiredService<IFlightBoardBroadcaster>();
                    var flightInfoService = scope.ServiceProvider.GetRequiredService<IFlightInfoService>();

                    foreach (var groupKey in FlightBoardHub.GetActiveGroups())
                    {
                        var parts = groupKey.Split(':');
                        if (parts.Length != 2) continue;

                        var airportCode = parts[0];
                        var direction = parts[1]; // "Departure" or "Arrival"

                        try
                        {
                            var cache = _flightCaches.GetOrAdd(groupKey, _ => new FlightBoardCacheHelper());

                            // Only initialize if controller hasn’t already seeded
                            if (!cache.Initialized)
                            {
                                var flights = direction == "Departure"
                                    ? await flightInfoService.GetDeparturesAsync(airportCode, _optionsApi.OffsetMinutes, _optionsApi.DurationMinutes)
                                    : await flightInfoService.GetArrivalsAsync(airportCode, _optionsApi.OffsetMinutes, _optionsApi.DurationMinutes);

                                cache.Initialize(flights, displayCount: _optionsPolling.DisplayCount, direction);

                                // Prevent immediate refresh
                                cache.LastApiRefresh = DateTimeOffset.UtcNow;

                                _logger.LogInformation("Initialized cache for {GroupKey} with {Count} flights", groupKey, flights.Count);
                            }

                            // 1. Rotate every tick (drop expired, add new up to DisplayCount)
                            cache.Rotate();

                            // Broadcast current display set
                            await broadcaster.BroadcastUpdateAsync(airportCode, direction, cache.DisplayedFlights);

                            // 2. Refresh API every ApiRefreshSeconds (to get fresh statuses/revised times)
                            if ((DateTimeOffset.UtcNow - cache.LastApiRefresh).TotalSeconds >= _optionsPolling.ApiRefreshSeconds)
                            {
                                var newFlights = cache.Direction == "Departure"
                                    ? await flightInfoService.GetDeparturesAsync(airportCode, _optionsApi.OffsetMinutes, _optionsApi.DurationMinutes)
                                    : await flightInfoService.GetArrivalsAsync(airportCode, _optionsApi.OffsetMinutes, _optionsApi.DurationMinutes);

                                cache.AddBatch(newFlights);
                                cache.LastApiRefresh = DateTimeOffset.UtcNow;

                                _logger.LogInformation("Time-based API refresh for {GroupKey}, added {Count} flights", groupKey, newFlights.Count);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error broadcasting flight board update for {AirportCode} {Direction}", airportCode, direction);
                        }
                    }

                    // Wait for next rotation
                    await Task.Delay(TimeSpan.FromSeconds(_optionsPolling.IntervalSeconds), token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Polling loop terminated unexpectedly.");
            }
        }
    }
}

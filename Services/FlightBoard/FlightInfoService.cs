using Microsoft.Extensions.Options;
using System.Text.Json;
using AvikstromPortfolio.Configuration;
using AvikstromPortfolio.Models.FlightInfo;

namespace AvikstromPortfolio.Services.FlightBoard
{
    public class FlightInfoService : IFlightInfoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ILogger<FlightInfoService> _logger;

        public FlightInfoService(HttpClient httpClient, IOptions<FlightInfoApiOptions> options, ILogger<FlightInfoService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            var config = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _baseUrl = config.BaseUrl ?? throw new ArgumentNullException("Missing FlightInfoService:BaseUrl.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves departure flights from the FlightInfo API for the given airport and time window.
        /// Logs errors and returns an empty list if the request fails.
        /// </summary>
        public async Task<List<FlightInfo>> GetDeparturesAsync(string iataCode, int offsetMinutes, int durationMinutes)
        {
            var endpoint = "/api/flightinfo/departures";
            var query = $"?iataCode={iataCode}&offsetMinutes={offsetMinutes}&durationMinutes={durationMinutes}";
            var url = $"{_baseUrl}{endpoint}{query}";

            try
            {
                _logger.LogInformation("Requesting departures from {Url}", url);
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var departures = JsonSerializer.Deserialize<List<FlightInfo>>(json);

                return departures ?? [];
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request to {Url} failed with status {StatusCode}.", url, ex.StatusCode);
                return [];
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Deserialization of departures response from {Url} failed.", url);
                return [];
            }
        }

        /// <summary>
        /// Retrieves arrival flights from the FlightInfo API for the given airport and time window.
        /// Logs errors and returns an empty list if the request fails.
        /// </summary>
        public async Task<List<FlightInfo>> GetArrivalsAsync(string iataCode, int fromMinutesOffset, int durationMinutes)
        {
            var endpoint = "/api/flightinfo/arrivals";
            var query = $"?iataCode={iataCode}&offsetMinutes={fromMinutesOffset}&durationMinutes={durationMinutes}";
            var url = $"{_baseUrl}{endpoint}{query}";

            try
            {
                _logger.LogInformation("Requesting arrivals from {Url}", url);
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var arrivals = JsonSerializer.Deserialize<List<FlightInfo>>(json);

                return arrivals ?? [];
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request to {Url} failed with status {StatusCode}.", url, ex.StatusCode);
                return [];
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Deserialization of arrivals response from {Url} failed.", url);
                return [];
            }
        }
    }
}

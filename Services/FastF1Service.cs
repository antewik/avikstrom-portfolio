using WikstromIT.Models.F1;
using WikstromIT.Services;
using WikstromIT.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;

public class FastF1Service : IFastF1Service
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration;

    public FastF1Service(HttpClient httpClient, IOptions<FastF1ApiOptions> options, IMemoryCache cache)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        var config = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _baseUrl = config.BaseUrl ?? throw new ArgumentNullException("Missing FastF1Api:BaseUrl.");
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _cacheDuration = TimeSpan.FromMinutes(config.CacheMinutes);
    }

    public async Task<RaceInfo?> GetNextRaceAsync()
    {
        const string cacheKey = "nextRaceInfo";

        if (_cache.TryGetValue(cacheKey, out RaceInfo? cachedRace))
        {
            return cachedRace;
        }

        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/raceinfo");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var raceInfo = JsonConvert.DeserializeObject<RaceInfo>(json);

            if (raceInfo != null)
            {
                _cache.Set(cacheKey, raceInfo, _cacheDuration);
            }

            return raceInfo;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request failed: {ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Deserialization failed: {ex.Message}");
            return null;
        }
    }

    public async Task<List<TeamStandings?>> GetTeamStandingsAsync()
    {
        const string cacheKey = "teamStandings";

        if (_cache.TryGetValue(cacheKey, out List<TeamStandings>? cachedTeamStandings))
        {
            return cachedTeamStandings ?? [];
        }

        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/teamstandings");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var teamStandings = JsonConvert.DeserializeObject<List<TeamStandings>>(json);

            if (teamStandings != null)
            {
                _cache.Set(cacheKey, teamStandings, _cacheDuration);
            }

            return teamStandings ?? [];
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request failed: {ex.Message}");
            return [];
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Deserialization failed: {ex.Message}");
            return [];
        }
    }

    public async Task<DriverStandingsWrapper?> GetDriverStandingsAsync()
    {
        const string cacheKey = "driverStandingsWrapper";

        if (_cache.TryGetValue(cacheKey, out DriverStandingsWrapper? cachedWrapper))
        {
            return cachedWrapper;
        }

        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/driverstandings");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var wrapper = JsonConvert.DeserializeObject<DriverStandingsWrapper>(json);

            if (wrapper != null)
            {
                _cache.Set(cacheKey, wrapper, _cacheDuration);
                return wrapper;
            }

            return wrapper;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request failed: {ex.Message}");
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Deserialization failed: {ex.Message}");
            return null;
        }
    }
}

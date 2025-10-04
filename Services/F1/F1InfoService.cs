using AvikstromPortfolio.Models.F1;
using AvikstromPortfolio.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using AvikstromPortfolio.Services.F1;

public class F1InfoService : IF1InfoService
{
    private readonly ILogger<F1InfoService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration;

    public F1InfoService(
        HttpClient httpClient,
        IOptions<F1InfoApiOptions> options,
        IMemoryCache cache,
        ILogger<F1InfoService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        var config = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _baseUrl = config.BaseUrl ?? throw new ArgumentNullException("Missing F1InfoApi:BaseUrl.");
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _cacheDuration = TimeSpan.FromMinutes(config.CacheMinutes);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<RaceInfo?> GetNextRaceAsync()
    {
        const string cacheKey = "nextRaceInfo";

        if (_cache.TryGetValue(cacheKey, out RaceInfo? cachedRace))
        {
            _logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
            return cachedRace;
        }

        try
        {
            _logger.LogInformation("Cache miss for {CacheKey}", cacheKey);
            var response = await _httpClient.GetAsync($"{_baseUrl}/nextrace");
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
            _logger.LogError(ex, "HTTP request to /raceinfo failed.");
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Deserialization of /raceinfo response failed.");
            return null;
        }
    }

    public async Task<List<TeamStandings?>> GetTeamStandingsAsync()
    {
        const string cacheKey = "teamStandings";

        if (_cache.TryGetValue(cacheKey, out List<TeamStandings>? cachedTeamStandings))
        {
            _logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
            return cachedTeamStandings ?? [];
        }

        try
        {
            _logger.LogInformation("Cache miss for {CacheKey}", cacheKey);
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
            _logger.LogError(ex, "HTTP request to /teamstandings failed.");
            return [];
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Deserialization of /teamstandings response failed.");
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
            _logger.LogInformation("Cache miss for {CacheKey}", cacheKey);
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
            _logger.LogError(ex, "HTTP request to /driverstandings failed.");
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Deserialization of /driverstandings response failed.");
            return null;
        }
    }
}

using Newtonsoft.Json;
using AvikstromPortfolio.DataModels;

namespace AvikstromPortfolio.Services.F1
{
    /// <summary>
    /// Maps Formula 1 driver nationalities to country information.
    /// Loads ISO country data from a JSON file and applies manual mappings
    /// for common nationality terms (e.g., "British" -> "GB").
    /// </summary>
    public class F1NationalityMapper : IF1NationalityMapper
    {
        private readonly Dictionary<string, CountryInfo> _nationalityToCountry;

        public F1NationalityMapper(IWebHostEnvironment env)
        {
            var jsonPath = Path.Combine(env.ContentRootPath, "App_Data", "Countries.json");
            _nationalityToCountry = LoadMappings(jsonPath);
        }

        /// <summary>
        /// Retrieves country information for the given nationality string.
        /// Returns null if the nationality is not recognized.
        /// </summary>
        public CountryInfo? GetCountryInfo(string? nationality)
        {
            if (string.IsNullOrWhiteSpace(nationality)) return null;
            return _nationalityToCountry.TryGetValue(nationality, out var info) ? info : null;
        }

        /// <summary>
        /// Loads country mappings from the specified JSON file and builds a lookup
        /// from nationality strings to <see cref="CountryInfo"/>.
        /// </summary>
        private Dictionary<string, CountryInfo> LoadMappings(string path)
        {
            if (!File.Exists(path)) return new();

            var json = File.ReadAllText(path);
            var countries = JsonConvert.DeserializeObject<List<CountryInfo>>(json);
            if (countries == null || countries.Count == 0) return new();

            var countryLookup = countries
                .Where(c => !string.IsNullOrWhiteSpace(c.IsoAlpha2))
                .ToDictionary(c => c.IsoAlpha2.ToLower(), c => c, StringComparer.OrdinalIgnoreCase);

            var manualMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "American", "us" },
                { "Argentine", "ar" },
                { "Australian", "au" },
                { "Austrian", "at" },
                { "Belgian", "be" },
                { "Brazilian", "br" },
                { "British", "gb" },
                { "Canadian", "ca" },
                { "Chinese", "cn" },
                { "Dutch", "nl" },
                { "Finnish", "fi" },
                { "French", "fr" },
                { "German", "de" },
                { "Italian", "it" },
                { "Japanese", "jp" },
                { "Mexican", "mx" },
                { "Monegasque", "mc" },
                { "New Zealander", "nz" },
                { "Spanish", "es" },
                { "Swedish", "se" },
                { "Swiss", "ch" },
                { "Thai", "th" }
            };

            var map = new Dictionary<string, CountryInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (var pair in manualMap)
            {
                if (countryLookup.TryGetValue(pair.Value, out var country))
                {
                    map[pair.Key] = country;
                }
            }

            return map;
        }
    }
}

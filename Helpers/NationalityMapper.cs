using Newtonsoft.Json;
using WikstromIT.DataModels;

namespace WikstromIT.Helpers
{
    public static class NationalityMapper
    {
        private static Dictionary<string, CountryInfo>? _nationalityToCountry;

        public static void Initialize(string jsonPath)
        {
            if (!File.Exists(jsonPath)) return;

            var json = File.ReadAllText(jsonPath);
            var countries = JsonConvert.DeserializeObject<List<CountryInfo>>(json);

            if (countries == null || countries.Count == 0) return;

            // Build lookup: ISO Alpha-2 → CountryInfo
            var countryLookup = countries
                .Where(c => !string.IsNullOrWhiteSpace(c.IsoAlpha2))
                .ToDictionary(c => c.IsoAlpha2.ToLower(), c => c, StringComparer.OrdinalIgnoreCase);

            // Manual mapping: Nationality → ISO Alpha-2
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

            _nationalityToCountry = new Dictionary<string, CountryInfo>(StringComparer.OrdinalIgnoreCase);

            foreach (var pair in manualMap)
            {
                if (countryLookup.TryGetValue(pair.Value, out var country))
                {
                    _nationalityToCountry[pair.Key] = country;
                }
            }
        }

        public static CountryInfo? GetCountryInfo(string? nationality)
        {
            if (string.IsNullOrWhiteSpace(nationality) || _nationalityToCountry == null)
                return null;

            return _nationalityToCountry.TryGetValue(nationality, out var info) ? info : null;
        }
    }
}

namespace AvikstromPortfolio.Services.F1
{
    public static class F1CodeMapper
    {
        private static readonly Dictionary<string, string> F1CodeOverrides = new(StringComparer.OrdinalIgnoreCase)
        {
            { "USA", "USA" }, // American
            { "ARG", "ARG" }, // Argentine
            { "AUS", "AUS" }, // Australian
            { "AUT", "AUT" }, // Austrian
            { "BEL", "BEL" }, // Belgian
            { "BRA", "BRA" }, // Brazilian
            { "GBR", "GBR" }, // British
            { "CAN", "CAN" }, // Canadian
            { "CHN", "CHN" }, // Chinese
            { "NLD", "NED" }, // Dutch
            { "FIN", "FIN" }, // Finnish
            { "FRA", "FRA" }, // French
            { "DEU", "GER" }, // German
            { "ITA", "ITA" }, // Italian
            { "JPN", "JAP" }, // Japanese
            { "MEX", "MEX" }, // Mexican
            { "MCO", "MON" }, // Monegasque
            { "NZL", "NZL" }, // New Zealander
            { "ESP", "SPA" }, // Spanish
            { "SWE", "SWE" }, // Swedish
            { "CHE", "SUI" }, // Swiss
            { "THA", "THA" }, // Thai
            { "ARE", "UAE" }, // United Arab Emirates
            { "KOR", "KOR" }  // Korean
        };

        public static string GetF1Code(string isoAlpha3)
        {
            if (string.IsNullOrWhiteSpace(isoAlpha3)) return "-";

            return F1CodeOverrides.TryGetValue(isoAlpha3.ToUpper(), out var f1Code)
                ? f1Code
                : isoAlpha3.ToUpper(); // fallback to ISO if no override
        }
    }
}
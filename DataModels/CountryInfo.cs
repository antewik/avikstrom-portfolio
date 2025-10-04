using Newtonsoft.Json;

namespace AvikstromPortfolio.DataModels
{
    public class CountryInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("isoAlpha2")]
        public string IsoAlpha2 { get; set; } = string.Empty;

        [JsonProperty("isoAlpha3")]
        public string IsoAlpha3 { get; set; } = string.Empty;
    }
}

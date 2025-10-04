using Newtonsoft.Json;

namespace AvikstromPortfolio.Models.F1
{
    public class TeamStandings
    {
        [JsonProperty("position")]
        public int? Position { get; set; }

        [JsonProperty("points")]
        public float? Points { get; set; }

        [JsonProperty("wins")]
        public int? Wins { get; set; }

        [JsonProperty("constructor_id")]
        public string? Constructor { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("nationality")]
        public string? Nationality { get; set; }

        [JsonProperty("url")]
        public string? ConstructorUrl { get; set; }

        public string CountryCode { get; set; } = "-";
    }
}

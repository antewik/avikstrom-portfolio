using Newtonsoft.Json;

namespace AvikstromPortfolio.Models.F1
{
    public class DriverStandingsWrapper
    {
        [JsonProperty("season")]
        public string? Season { get; set; }

        [JsonProperty("standings")]
        public List<DriverStanding>? DriverStandings {get; set; }
    }
    public class DriverStanding
    {
        [JsonProperty("position")]
        public int? Position { get; set; }

        [JsonProperty("points")]
        public float? Points { get; set; }

        [JsonProperty("wins")]
        public int? Wins { get; set; }

        [JsonProperty("driver_id")]
        public string? DriverId { get; set; }

        [JsonProperty("given_name")]
        public string? FirstName { get; set; }

        [JsonProperty("family_name")]
        public string? LastName { get; set; }

        [JsonProperty("nationality")]
        public string? Nationality { get; set; }

        [JsonProperty("permanentNumber")]
        public string? DriverNumber { get; set; }

        [JsonProperty("url")]
        public string? DriverUrl { get; set; }

        [JsonProperty("constructor")]
        public string? Constructor { get; set; }

        public string CountryCode { get; set; } = "-";
    }
}

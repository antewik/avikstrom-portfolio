using System.Text.Json.Serialization;

namespace AvikstromPortfolio.Models.FlightInfo
{
    public class FlightInfo
    {
        [JsonPropertyName("flightNumber")]
        public string FlightNumber { get; set; }

        [JsonPropertyName("airline")]
        public string Airline { get; set; }

        [JsonPropertyName("timeScheduled")]
        public FlightTimestamp TimeScheduled { get; set; }

        [JsonPropertyName("timeRevised")]
        public FlightTimestamp TimeRevised { get; set; }

        [JsonPropertyName("timeRunway")]
        public FlightTimestamp TimeRunway { get; set; }

        [JsonPropertyName("airport")]
        public string Airport { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("gate")]
        public string Gate { get; set; }

        [JsonPropertyName("terminal")]
        public string Terminal { get; set; }

        [JsonPropertyName("aircraftModel")]
        public string AircraftModel { get; set; }

        // Direction context: "Departure" or "Arrival"
        public string Direction { get; set; }
    }

    public class FlightTimestamp
    {
        [JsonPropertyName("utc")]
        public DateTimeOffset? Utc { get; set; }

        [JsonPropertyName("local")]
        public DateTimeOffset? Local { get; set; }
    }
}

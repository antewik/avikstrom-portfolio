using Newtonsoft.Json;

namespace AvikstromPortfolio.Models.F1
{
    public class RaceInfo
    {

        [JsonProperty("country")]
        public string? Country { get; set; }

        [JsonProperty("event_format")]
        public string? EventFormat { get; set; }

        [JsonProperty("f1_api_support")]
        public string? F1ApiSupport { get; set; }

        [JsonProperty("location")]
        public string? Location { get; set; }

        [JsonProperty("next_race")]
        public string? NextRaceName { get; set; }

        [JsonProperty("official_event_name")]
        public string? OfficialEventName { get; set; }

        [JsonProperty("round")]
        public int? Round { get; set; }


        [JsonProperty("sessions")]
        public List<RaceSession> Sessions { get; set; } = new();


        [JsonProperty("message")]
        public string? Message { get; set; }

        // Countdown
        public int CountdownDays { get; set; }
        public int CountdownHours { get; set; }
        public int CountdownMinutes { get; set; }
        public int CountdownSeconds { get; set; }


        public class RaceSession
        {
            [JsonProperty("name")]
            public string Label { get; set; } = string.Empty;

            [JsonProperty("local")]
            public SessionTime Local { get; set; } = new();

            [JsonProperty("utc")]
            public SessionTime Utc { get; set; } = new();

            public DateTime? TrackDateTime => Combine(Local.Day, Local.Time);
            public DateTime? UtcDateTime => Combine(Utc.Day, Utc.Time);

            private static DateTime? Combine(DateOnly? day, TimeOnly? time)
            {
                if (day.HasValue && time.HasValue)
                {
                    var combined = day.Value.ToDateTime(time.Value);
                    return DateTime.SpecifyKind(combined, DateTimeKind.Utc);
                }
                return null;
            }
        }

        public class SessionTime
        {
            [JsonProperty("day")]
            public DateOnly? Day { get; set; }

            [JsonProperty("time")]
            public TimeOnly? Time { get; set; }

        }
    }
}

using Newtonsoft.Json;

namespace WikstromIT.Models.F1
{
    public class RaceInfo
    {
        [JsonProperty("next_race")]
        public string? NextRaceName { get; set; }

        [JsonProperty("location")]
        public string? Location { get; set; }

        [JsonProperty("country")]
        public string? Country { get; set; }

        [JsonProperty("official_event_name")]
        public string? OfficialEventName { get; set; }

        [JsonProperty("round")]
        public int? Round { get; set; }

        [JsonProperty("f1_api_support")]
        public string? F1ApiSupport { get; set; }

        // Practice 1
        [JsonProperty("practice1_day")]
        public DateOnly? Practice1Day { get; set; }

        [JsonProperty("practice1_time")]
        public TimeOnly? Practice1Time { get; set; }

        [JsonProperty("practice1_day_utc")]
        public DateOnly? Practice1DayUtc { get; set; }

        [JsonProperty("practice1_time_utc")]
        public TimeOnly? Practice1TimeUtc { get; set; }

        // Practice 2
        [JsonProperty("practice2_day")]
        public DateOnly? Practice2Day { get; set; }

        [JsonProperty("practice2_time")]
        public TimeOnly? Practice2Time { get; set; }

        [JsonProperty("practice2_day_utc")]
        public DateOnly? Practice2DayUtc { get; set; }

        [JsonProperty("practice2_time_utc")]
        public TimeOnly? Practice2TimeUtc { get; set; }

        // Practice 3
        [JsonProperty("practice3_day")]
        public DateOnly? Practice3Day { get; set; }

        [JsonProperty("practice3_time")]
        public TimeOnly? Practice3Time { get; set; }

        [JsonProperty("practice3_day_utc")]
        public DateOnly? Practice3DayUtc { get; set; }

        [JsonProperty("practice3_time_utc")]
        public TimeOnly? Practice3TimeUtc { get; set; }

        // Qualifying
        [JsonProperty("qualifying_day")]
        public DateOnly? QualifyingDay { get; set; }

        [JsonProperty("qualifying_time")]
        public TimeOnly? QualifyingTime { get; set; }

        [JsonProperty("qualifying_day_utc")]
        public DateOnly? QualifyingDayUtc { get; set; }

        [JsonProperty("qualifying_time_utc")]
        public TimeOnly? QualifyingTimeUtc { get; set; }

        // Race
        [JsonProperty("race_day")]
        public DateOnly? RaceDay { get; set; }

        [JsonProperty("race_time")]
        public TimeOnly? RaceTime { get; set; }

        [JsonProperty("race_day_utc")]
        public DateOnly? RaceDayUtc { get; set; }

        [JsonProperty("race_time_utc")]
        public TimeOnly? RaceTimeUtc { get; set; }

        // Countdown
        public int CountdownDays { get; set; }
        public int CountdownHours { get; set; }
        public int CountdownMinutes { get; set; }
        public int CountdownSeconds { get; set; }

        // Sessions list using pre-supplied UTC values
        public List<SessionInfo> Sessions => new()
        {
            new SessionInfo { Label = "Practice 1", UtcDateTime = Combine(Practice1DayUtc, Practice1TimeUtc) },
            new SessionInfo { Label = "Practice 2", UtcDateTime = Combine(Practice2DayUtc, Practice2TimeUtc) },
            new SessionInfo { Label = "Practice 3", UtcDateTime = Combine(Practice3DayUtc, Practice3TimeUtc) },
            new SessionInfo { Label = "Qualifying", UtcDateTime = Combine(QualifyingDayUtc, QualifyingTimeUtc) },
            new SessionInfo { Label = "Race", UtcDateTime = Combine(RaceDayUtc, RaceTimeUtc) }
        };

        private DateTime? Combine(DateOnly? day, TimeOnly? time)
        {
            if (day.HasValue && time.HasValue)
            {
                var combined = day.Value.ToDateTime(time.Value);
                return DateTime.SpecifyKind(combined, DateTimeKind.Utc); // Ensure it's treated as UTC
            }
            return null;
        }
    }

    public class SessionInfo
    {
        public string Label { get; set; } = string.Empty;
        public DateTime? UtcDateTime { get; set; }
    }
}

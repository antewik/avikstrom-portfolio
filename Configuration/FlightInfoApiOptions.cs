namespace AvikstromPortfolio.Configuration
{
    public class FlightInfoApiOptions
    {
        // Base URL for FlightInfo API
        public string BaseUrl { get; set; } = string.Empty;

        // Offset in minutes from current time to start fetching flights
        public int OffsetMinutes { get; set; } = -10;

        // Duration in minutes for which to fetch flights
        public int DurationMinutes { get; set; } = 180;
    }
}

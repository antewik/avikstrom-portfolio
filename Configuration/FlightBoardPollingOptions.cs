namespace AvikstromPortfolio.Configuration
{
    public class FlightBoardPollingOptions
    {
        // Rotation tick (board updates every X seconds)
        public int IntervalSeconds { get; set; } = 10;

        // How many flights to show at once
        public int DisplayCount { get; set; } = 20;

        // How often to refresh from API (in seconds)
        public int ApiRefreshSeconds { get; set; } = 120;
    }
}

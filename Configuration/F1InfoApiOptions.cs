namespace AvikstromPortfolio.Configuration
{
    public class F1InfoApiOptions
    {
        // Base URL for the F1 Info API
        public string BaseUrl { get; set; } = string.Empty;

        // Cache duration in minutes for API responses
        public int CacheMinutes { get; set; } = 30;
    }
}

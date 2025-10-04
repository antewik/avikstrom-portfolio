namespace WikstromIT.Configuration
{
    public class FastF1ApiOptions
    {
        public string BaseUrl { get; set; } = string.Empty;

        public int CacheMinutes { get; set; } = 30;
    }
}

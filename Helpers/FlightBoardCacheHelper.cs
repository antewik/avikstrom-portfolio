using AvikstromPortfolio.Models.FlightInfo;

namespace AvikstromPortfolio.Helpers
{
    /// <summary>
    /// Helper for managing cached flight board data.
    /// Tracks displayed flights, remaining flights (in batch), and handles rotation logic
    /// to keep the board fresh.
    /// </summary>
    public class FlightBoardCacheHelper
    {
        public Queue<FlightInfo> RemainingFlights { get; private set; } = [];
        public List<FlightInfo> DisplayedFlights { get; private set; } = [];
        public bool Initialized { get; private set; }
        public string Direction { get; private set; } = "Departure";
        public string AirportCode { get; private set; } = string.Empty;
        public DateTimeOffset LastApiRefresh { get; set; } = DateTimeOffset.MinValue;

        public int MaxFlightsShown = 20;

        public void Initialize(IEnumerable<FlightInfo> flights, int displayCount, string direction, string airportCode = "")
        {
            Direction = direction;
            AirportCode = airportCode;

            var list = flights?.ToList() ?? new List<FlightInfo>();
            DisplayedFlights = list.Take(displayCount).ToList();
            RemainingFlights = new Queue<FlightInfo>(list.Skip(displayCount));

            MaxFlightsShown = displayCount;
            Initialized = true;

            LastApiRefresh = DateTimeOffset.UtcNow;
        }

        public void Rotate()
        {
            var cutoff = DateTimeOffset.UtcNow.AddMinutes(-10);

            // Drop flights whose display time is older than cutoff
            DisplayedFlights.RemoveAll(f =>
            {
                var display = GetRevisedUtc(f) ?? GetScheduledUtc(f);
                return display.HasValue && display.Value < cutoff;
            });

            // Fill up next flights
            while (DisplayedFlights.Count < MaxFlightsShown && RemainingFlights.Any())
            {
                DisplayedFlights.Add(RemainingFlights.Dequeue());
            }
        }

        public void AddBatch(IEnumerable<FlightInfo> flights)
        {
            if (flights == null) return;

            foreach (var f in flights)
                RemainingFlights.Enqueue(f);
        }

        private static DateTimeOffset? GetRevisedUtc(FlightInfo f)
        {
            var ts = f.TimeRevised;
            return ts?.Utc ?? ts?.Local;
        }

        private static DateTimeOffset? GetScheduledUtc(FlightInfo f)
        {
            var ts = f.TimeScheduled;
            return ts?.Utc ?? ts?.Local;
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using AvikstromPortfolio.Configuration;
using AvikstromPortfolio.Hubs;
using AvikstromPortfolio.Models.FlightInfo;

namespace AvikstromPortfolio.Services.FlightBoard
{
    /// <summary>
    /// Broadcasts flight board updates to connected SignalR clients.
    /// Renders flight data into HTML and sends it to the appropriate airport/direction group.
    /// </summary>
    public class FlightBoardBroadcaster : IFlightBoardBroadcaster
    {
        private readonly IHubContext<FlightBoardHub> _hubContext;
        private readonly IFlightBoardPartialRenderer _viewRenderService;
        private readonly FlightInfoApiOptions _options;
        private readonly ILogger<FlightBoardBroadcaster> _logger;

        public FlightBoardBroadcaster(
            IHubContext<FlightBoardHub> hubContext,
            IOptions<FlightInfoApiOptions> options,
            ILogger<FlightBoardBroadcaster> logger,
            IFlightBoardPartialRenderer viewRenderService)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _viewRenderService = viewRenderService ?? throw new ArgumentNullException(nameof(viewRenderService));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Broadcasts a flight update to clients in the specified airport/direction group.
        /// Renders the flights into HTML before sending. Logs warnings if no flights are available.
        /// </summary>
        public async Task BroadcastUpdateAsync(string iataCode, string direction, IReadOnlyList<FlightInfo> flights)
        {
            _logger.LogInformation("Preparing to broadcast update for {IataCode} {Direction}", iataCode, direction);

            try
            {
                if (flights == null || flights.Count == 0)
                {
                    _logger.LogWarning("No flights to broadcast for {IataCode} {Direction}", iataCode, direction);
                    return;
                }

                // Render the provided flights into HTML
                var html = await _viewRenderService.RenderToStringAsync(flights);

                // Use the composite group key (airport:direction)
                var groupKey = $"{iataCode}:{direction}";
                await _hubContext.Clients.Group(groupKey).SendAsync("ReceiveFlightUpdate", html);

                _logger.LogInformation("Broadcasted update for {IataCode} {Direction} with {Count} flights",
                    iataCode, direction, flights.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error broadcasting flight board update for {IataCode} {Direction}", iataCode, direction);
            }
        }
    }
}

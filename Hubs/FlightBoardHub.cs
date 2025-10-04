using Microsoft.AspNetCore.SignalR;
using AvikstromPortfolio.Services.FlightBoard;
using System.Collections.Concurrent;

namespace AvikstromPortfolio.Hubs
{
    /// <summary>
    /// SignalR hub for broadcasting live flight board updates to connected clients.
    /// Polling starts when at least one client joins an airport group, and stops when all groups are empty.
    /// </summary>
    public class FlightBoardHub : Hub
    {
        private readonly ILogger<FlightBoardHub> _logger;
        private readonly IFlightBoardPollingActivator _pollingActivator;

        // Tracks group membership: airportCode -> set of connectionIds
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _groupMembers = new();

        public FlightBoardHub(ILogger<FlightBoardHub> logger, IFlightBoardPollingActivator pollingActivator)
        {
            _logger = logger;
            _pollingActivator = pollingActivator;
        }

        /// <summary>
        /// Adds the current client connection to the specified airport group.
        /// Starts polling if this is the first active group.
        /// </summary>
        public async Task JoinAirportGroup(string airportCode, string direction)
        {
            var connectionId = Context.ConnectionId;
            var groupKey = $"{airportCode}:{direction}";

            await Groups.AddToGroupAsync(connectionId, groupKey);

            var group = _groupMembers.GetOrAdd(groupKey, _ => new ConcurrentDictionary<string, byte>());
            group.TryAdd(connectionId, 0);

            _logger.LogInformation("Client {ConnectionId} joined group {GroupKey}", connectionId, groupKey);

            // Start polling if this is the first active group
            if (_groupMembers.Count == 1 && group.Count == 1)
            {
                _logger.LogInformation("First airport group joined. Starting polling.");
                _pollingActivator.TryStartPolling();
            }
        }

        /// <summary>
        /// Removes the current client connection from the specified airport group.
        /// Cleans up empty groups and stops polling if no groups remain.
        /// </summary>
        public async Task LeaveAirportGroup(string airportCode, string direction)
        {
            var connectionId = Context.ConnectionId;
            var groupKey = $"{airportCode}:{direction}";

            await Groups.RemoveFromGroupAsync(connectionId, groupKey);

            if (_groupMembers.TryGetValue(groupKey, out var group))
            {
                if (group.TryRemove(connectionId, out _))
                {
                    _logger.LogInformation("Client {ConnectionId} left group {GroupKey}", connectionId, groupKey);
                }

                // Cleanup empty groups
                if (group.IsEmpty)
                {
                    _groupMembers.TryRemove(groupKey, out _);
                    _logger.LogInformation("Group {GroupKey} is now empty and removed", groupKey);

                    // If no groups remain, stop polling
                    if (_groupMembers.IsEmpty)
                    {
                        _logger.LogInformation("No active airport groups. Stopping polling.");
                        _pollingActivator.StopPolling();
                    }
                }
            }
        }

        /// <summary>
        /// Handles client disconnection by removing them from all groups.
        /// Ensures polling stops when no groups are active.
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;

            foreach (var group in _groupMembers.Keys)
            {
                if (_groupMembers[group].TryRemove(connectionId, out _))
                {
                    _logger.LogInformation("Client {ConnectionId} removed from group {Group}", connectionId, group);
                }

                // Cleanup empty groups
                if (_groupMembers[group].IsEmpty)
                {
                    _groupMembers.TryRemove(group, out _);
                    _logger.LogInformation("Group {Group} is now empty and removed", group);
                }
            }

            // If no groups remain, stop polling
            if (_groupMembers.IsEmpty)
            {
                _logger.LogInformation("No active airport groups. Stopping polling.");
                _pollingActivator.StopPolling();
            }

            await base.OnDisconnectedAsync(exception);
        }

        public static IEnumerable<string> GetActiveGroups()
        {
            return _groupMembers.Keys;
        }

    }
}

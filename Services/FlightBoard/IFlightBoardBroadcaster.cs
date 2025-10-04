using AvikstromPortfolio.Models.FlightInfo;

namespace AvikstromPortfolio.Services.FlightBoard
{
    /// <summary>
    /// Defines the contract for broadcasting flight board updates to connected clients.
    /// Sends updated flight information for a specific airport and direction.
    /// </summary>
    public interface IFlightBoardBroadcaster
    {
        Task BroadcastUpdateAsync(string iataCode, string direction, IReadOnlyList<FlightInfo> flights);
    }
}

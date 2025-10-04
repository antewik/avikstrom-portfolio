namespace AvikstromPortfolio.Services.FlightBoard
{
    /// <summary>
    /// Defines the contract for rendering flight board views to HTML strings.
    /// Used for generating partial views for SignalR updates or AJAX responses.
    /// </summary>
    public interface IFlightBoardPartialRenderer
    {
        Task<string> RenderToStringAsync(object model);
    }
}

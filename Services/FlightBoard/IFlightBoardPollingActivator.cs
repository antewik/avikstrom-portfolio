namespace AvikstromPortfolio.Services.FlightBoard
{
    /// <summary>
    /// Defines the contract for controlling flight board polling.
    /// Responsible for starting and stopping background polling of flight data.
    /// </summary>
    public interface IFlightBoardPollingActivator
    {
        void TryStartPolling();
        void StopPolling();
    }
}
using AvikstromPortfolio.Models.FlightInfo;

namespace AvikstromPortfolio.Services.FlightBoard
{
    /// <summary>
    /// Defines the contract for accessing flight arrival and departure data from the FlightInfo API.
    /// </summary>
    public interface IFlightInfoService
    {
        /// <summary>
        /// Retrieves a list of scheduled and live departure flights for a specific airport and terminal within a given time window.
        /// </summary>
        /// <param name="iataCode">The IATA code of the airport (e.g., "LAX").</param>
        /// <param name="fromMinutesOffset">The start of the time window, in minutes offset from the current UTC time (e.g., -60 for 1 hour ago).</param>
        /// <param name="toMinutesOffset">The end of the time window, in minutes offset from the current UTC time (e.g., 180 for 3 hours ahead).</param>
        /// <returns>A list of <see cref="FlightInfo"/> objects representing departure flights, or an empty list if none are found.</returns>
        Task<List<FlightInfo>> GetDeparturesAsync(string iataCode, int offsetMinutes, int durationMinutes);

        /// <summary>
        /// Retrieves a list of scheduled and live arrival flights for a specific airport and terminal within a given time window.
        /// </summary>
        /// <param name="iataCode">The IATA code of the airport (e.g., "LAX").</param>
        /// <param name="fromMinutesOffset">The start of the time window, in minutes offset from the current UTC time (e.g., -60 for 1 hour ago).</param>
        /// <param name="toMinutesOffset">The end of the time window, in minutes offset from the current UTC time (e.g., 180 for 3 hours ahead).</param>
        /// <returns>A list of <see cref="FlightInfo"/> objects representing arrival flights, or an empty list if none are found.</returns>
        Task<List<FlightInfo>> GetArrivalsAsync(string iataCode, int offsetMinutes, int durationMinutes);
    }
}
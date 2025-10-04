using WikstromIT.Models.F1;

namespace WikstromIT.Services
{
    /// <summary>
    /// Defines the contract for accessing FastF1 race data.
    /// </summary>
    public interface IFastF1Service
    {
        /// <summary>
        /// Retrieves information about the next scheduled Formula 1 race.
        /// </summary>
        /// <returns>A <see cref="RaceInfo"/> object containing race details, or null if unavailable.</returns>
        Task<RaceInfo?> GetNextRaceAsync();

        /// <summary>
        /// Retrieves information about the teams standings
        /// </summary>
        /// <returns>A <see cref="TeamStandings"/> object containing team standings, or null if unavailable.</returns>
        Task<List<TeamStandings?>> GetTeamStandingsAsync();

        /// <summary>
        /// Retrieves information about the drivers standings
        /// </summary>
        /// <returns>A <see cref="DriverStanding"/> object containing driver standings, or null if unavailable.</returns>
        Task<DriverStandingsWrapper?> GetDriverStandingsAsync();
    }
}

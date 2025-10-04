using AvikstromPortfolio.DataModels;

namespace AvikstromPortfolio.Services.F1
{
    /// <summary>
    /// Defines the contract for mapping driver nationalities to country information.
    /// </summary>
    public interface IF1NationalityMapper
    {
        /// <summary>
        /// Retrieves country information for the given nationality string
        /// </summary>
        CountryInfo? GetCountryInfo(string? nationality);
    }
}
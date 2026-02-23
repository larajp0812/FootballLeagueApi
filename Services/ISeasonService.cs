using FootballLeagueApi.Models;

namespace FootballLeagueApi.Services
{
    /// <summary>
    /// ISeasonService Interface - Defines business logic operations for seasons
    /// 
    /// This interface defines the contract for season-related business operations.
    /// Seasons represent the time periods during which league matches are played.
    /// </summary>
    public interface ISeasonService
    {
        /// <summary>
        /// Retrieve all seasons from the database
        /// Returns an enumerable of all Season objects
        /// </summary>
        Task<IEnumerable<Season>> GetAllAsync();

        /// <summary>
        /// Retrieve a specific season by its ID
        /// Returns the Season if found, or null if not found
        /// </summary>
        Task<Season?> GetByIdAsync(int id);

        /// <summary>
        /// Create a new season
        /// Accepts a Season with Name, StartDate, and EndDate
        /// Returns the created Season with its auto-generated SeasonId
        /// </summary>
        Task<Season> CreateAsync(Season season);

        /// <summary>
        /// Update an existing season (e.g., extend end date, change name)
        /// Returns true if successful, false if ID mismatch or season not found
        /// </summary>
        Task<bool> UpdateAsync(int id, Season season);

        /// <summary>
        /// Delete a season by its ID
        /// Returns true if successful, false if season not found
        /// Note: May be prevented if season has associated matches
        /// </summary>
        Task<bool> DeleteAsync(int id);
    }
}

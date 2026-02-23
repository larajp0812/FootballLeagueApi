using FootballLeagueApi.Models;

namespace FootballLeagueApi.Services
{
    /// <summary>
    /// IMatchService Interface - Defines business logic operations for matches
    /// 
    /// This interface defines the contract for match-related business operations.
    /// Matches represent games between two teams with scores and associated events.
    /// This service likely includes validation (e.g., home team != away team).
    /// </summary>
    public interface IMatchService
    {
        /// <summary>
        /// Retrieve all matches from the database
        /// Returns an enumerable of all Match objects
        /// </summary>
        Task<IEnumerable<Match>> GetAllAsync();

        /// <summary>
        /// Retrieve a specific match by its ID
        /// Returns the Match if found, or null if not found
        /// </summary>
        Task<Match?> GetByIdAsync(int id);

        /// <summary>
        /// Create a new match
        /// Accepts a Match with HomeTeamId, AwayTeamId, SeasonId, VenueId, and KickoffTime
        /// May throw exceptions if business rules are violated (e.g., team not found)
        /// Returns the created Match with its auto-generated MatchId
        /// </summary>
        Task<Match> CreateAsync(Match match);

        /// <summary>
        /// Update an existing match (e.g., record final score)
        /// Returns true if successful, false if ID mismatch or match not found
        /// </summary>
        Task<bool> UpdateAsync(int id, Match match);

        /// <summary>
        /// Delete a match by its ID
        /// Returns true if successful, false if match not found
        /// </summary>
        Task<bool> DeleteAsync(int id);
    }
}

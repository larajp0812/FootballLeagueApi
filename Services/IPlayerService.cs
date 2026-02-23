using FootballLeagueApi.Models;

namespace FootballLeagueApi.Services
{
    /// <summary>
    /// IPlayerService Interface - Defines business logic operations for players
    /// 
    /// This interface defines the contract for player-related business operations.
    /// Services implement CRUD operations plus any business logic specific to players.
    /// All methods are async for non-blocking database I/O.
    /// </summary>
    public interface IPlayerService
    {
        /// <summary>
        /// Retrieve all players from the database
        /// Returns an enumerable of all Player objects
        /// </summary>
        Task<IEnumerable<Player>> GetAllAsync();

        /// <summary>
        /// Retrieve a specific player by their ID
        /// Returns the Player if found, or null if not found
        /// </summary>
        Task<Player?> GetByIdAsync(int id);

        /// <summary>
        /// Create a new player
        /// Accepts a Player with FullName, ShirtNumber, Position, and TeamId
        /// Returns the created Player with its auto-generated PlayerId
        /// </summary>
        Task<Player> CreateAsync(Player player);

        /// <summary>
        /// Update an existing player
        /// Returns true if successful, false if ID mismatch or player not found
        /// </summary>
        Task<bool> UpdateAsync(int id, Player player);

        /// <summary>
        /// Delete a player by their ID
        /// Returns true if successful, false if player not found
        /// </summary>
        Task<bool> DeleteAsync(int id);
    }
}

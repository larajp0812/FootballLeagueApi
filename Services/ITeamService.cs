using FootballLeagueApi.Models;

namespace FootballLeagueApi.Services
{
    /// <summary>
    /// ITeamService Interface - Defines business logic operations for teams
    /// 
    /// This interface defines the contract for team-related business operations.
    /// Services implement business logic like validation and data coordination.
    /// The controller depends on this interface, not the concrete implementation,
    /// allowing for easy testing with mock implementations.
    /// </summary>
    public interface ITeamService
    {
        /// <summary>
        /// Retrieve all teams from the database
        /// Returns an enumerable of all Team objects
        /// </summary>
        Task<IEnumerable<Team>> GetAllAsync();

        /// <summary>
        /// Retrieve a specific team by its ID
        /// Returns the Team if found, or null if not found
        /// </summary>
        Task<Team?> GetByIdAsync(int id);

        /// <summary>
        /// Create a new team in the database
        /// Accepts a Team model with Name, Coach, and FoundedYear
        /// Returns the created Team with its auto-generated TeamId
        /// </summary>
        Task<Team> CreateAsync(Team team);

        /// <summary>
        /// Update an existing team
        /// Parameters:
        ///   id - The TeamId of the team to update
        ///   team - The updated Team object
        /// Returns true if successful, false if the ID doesn't match or team not found
        /// </summary>
        Task<bool> UpdateAsync(int id, Team team);

        /// <summary>
        /// Delete a team by its ID
        /// Returns true if successful, false if team not found
        /// Note: Deletion is prevented if the team has scheduled matches (referential integrity)
        /// </summary>
        Task<bool> DeleteAsync(int id);
    }
}

using FootballLeagueApi.Models;

namespace FootballLeagueApi.Repositories
{
    /// <summary>
    /// ITeamRepository Interface - Defines data access operations for teams
    /// 
    /// This interface abstracts all database operations for teams.
    /// By programming to this interface rather than a concrete class,
    /// we create a layer between business logic and database code.
    /// This allows easy testing with mock repositories.
    /// </summary>
    public interface ITeamRepository
    {
        /// <summary>
        /// Retrieve all teams from the database
        /// Returns all Team entities in the database
        /// </summary>
        Task<IEnumerable<Team>> GetAllAsync();

        /// <summary>
        /// Retrieve a specific team by its ID
        /// Returns the Team if found, or null if not found
        /// </summary>
        Task<Team?> GetByIdAsync(int id);

        /// <summary>
        /// Add a new team to the database context
        /// Note: Changes are not saved until SaveChangesAsync is called
        /// This allows batching multiple operations before committing
        /// </summary>
        Task AddAsync(Team team);

        /// <summary>
        /// Update an existing team (mark it as modified)
        /// The actual database update happens when SaveChangesAsync is called
        /// </summary>
        Task UpdateAsync(Team team);

        /// <summary>
        /// Delete a team from the database context
        /// Changes are persisted when SaveChangesAsync is called
        /// </summary>
        Task DeleteAsync(Team team);

        /// <summary>
        /// Check if a team has any players assigned
        /// </summary>
        Task<bool> HasPlayersAsync(int teamId);

        /// <summary>
        /// Check if a team is referenced by any matches (home or away)
        /// </summary>
        Task<bool> HasMatchesAsync(int teamId);

        /// <summary>
        /// Persist all pending changes to the database
        /// Sends all Add/Update/Delete operations to the database
        /// Returns true if successful, false if there were errors
        /// </summary>
        Task<bool> SaveChangesAsync();
    }
}

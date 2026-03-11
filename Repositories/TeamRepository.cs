using FootballLeagueApi.Models;
using FootballLeagueApi.Data;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueApi.Repositories
{
    /// <summary>
    /// TeamRepository - Implements data access operations for teams
    /// 
    /// This class directly interfaces with Entity Framework Core and the database.
    /// It implements ITeamRepository, providing concrete implementations of all
    /// data access methods for teams.
    /// </summary>
    public class TeamRepository : ITeamRepository
    {
        /// <summary>
        /// The Entity Framework Core database context
        /// Provides access to the database and its entities
        /// Made readonly to prevent accidental reassignment
        /// </summary>
        private readonly LeagueContext _context;

        /// <summary>
        /// Constructor accepting LeagueContext through dependency injection
        /// The context is provided by the DI container configured in Program.cs
        /// </summary>
        public TeamRepository(LeagueContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieve all teams from the database
        /// Uses LINQ to EF Core to query the Teams table
        /// ToListAsync executes the query asynchronously
        /// </summary>
        public async Task<IEnumerable<Team>> GetAllAsync()
        {
            return await _context.Teams.ToListAsync();
        }

        /// <summary>
        /// Retrieve a specific team by ID
        /// FindAsync is an efficient method that uses the primary key
        /// Returns the Team if found, null if not found
        /// </summary>
        public async Task<Team?> GetByIdAsync(int id)
        {
            return await _context.Teams.FindAsync(id);
        }

        /// <summary>
        /// Add a new team to the database context
        /// This marks the team as "Added" in the change tracker
        /// The team is not actually saved until SaveChangesAsync is called
        /// </summary>
        public async Task AddAsync(Team team)
        {
            await _context.Teams.AddAsync(team);
        }

        /// <summary>
        /// Mark a team as modified in the database context
        /// This tells EF Core to prepare an UPDATE statement
        /// Changes aren't persisted until SaveChangesAsync is called
        /// </summary>
        public async Task UpdateAsync(Team team)
        {
            _context.Teams.Update(team);
        }

        /// <summary>
        /// Mark a team for deletion from the database context
        /// This tells EF Core to prepare a DELETE statement
        /// The team is actually deleted when SaveChangesAsync is called
        /// </summary>
        public async Task DeleteAsync(Team team)
        {
            _context.Teams.Remove(team);
        }

        public async Task<bool> HasPlayersAsync(int teamId)
        {
            return await _context.Players.AnyAsync(player => player.TeamId == teamId);
        }

        public async Task<bool> HasMatchesAsync(int teamId)
        {
            return await _context.Matches.AnyAsync(match =>
                match.HomeTeamId == teamId || match.AwayTeamId == teamId);
        }

        /// <summary>
        /// Persist all pending changes to the database
        /// This sends all Add/Update/Delete operations to SQL Server
        /// Returns true if at least one change was saved, false otherwise
        /// SaveChangesAsync returns the number of affected rows
        /// </summary>
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

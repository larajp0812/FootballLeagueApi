using FootballLeagueApi.Models;
using FootballLeagueApi.Repositories;

namespace FootballLeagueApi.Services
{
    /// <summary>
    /// TeamService - Business Logic Layer for Teams
    /// 
    /// Service Layer Responsibilities:
    /// 1. Coordinate calls to repositories (data layer)
    /// 2. Implement business rules and validation
    /// 3. Handle transactions (SaveChangesAsync)
    /// 4. Log operations for debugging and auditing
    /// 5. Return results to controllers
    /// 
    /// Why a separate service layer?
    /// - Controllers should not directly access repositories (separation of concerns)
    /// - Business logic stays in services, not scattered in controllers
    /// - Easy to test: mock ITeamRepository and test service logic
    /// - Easy to modify: business rules in one place
    /// - Easy to reuse: service methods can be called from multiple controllers
    /// 
    /// Example: Create a team
    /// 1. Controller receives HTTP request with team data
    /// 2. Controller calls service.CreateAsync(team)
    /// 3. Service logs operation
    /// 4. Service calls repository.AddAsync(team) - stages in EF Core memory
    /// 5. Service calls repository.SaveChangesAsync() - executes SQL INSERT
    /// 6. Service logs success and returns team
    /// 7. Controller returns HTTP 201 Created response
    /// 
    /// Dependency Injection:
    /// - ITeamRepository: Injected in constructor, implements repository pattern
    /// - ILogger&lt;TeamService&gt;: Injected in constructor, logs operations
    /// - Any other service could be injected if needed (e.g., IEmailService, IPlayerService)
    /// </summary>
    public class TeamService : ITeamService
    {
        /// <summary>
        /// Repository for data access
        /// Injected through constructor
        /// Used for Add, Update, Delete, Get operations
        /// </summary>
        private readonly ITeamRepository _repo;
        
        /// <summary>
        /// Logger for debugging and auditing
        /// Injected through constructor
        /// Used to log operations: LogDebug, LogInformation, LogWarning, LogError
        /// </summary>
        private readonly ILogger<TeamService> _logger;

        /// <summary>
        /// Constructor - Dependency Injection
        /// DI Container (in Program.cs) provides ITeamRepository and ILogger instances
        /// 
        /// Example of dependency injection in action:
        /// var service = new TeamService(repository, logger);
        /// // DI container creates these for you:
        /// var repository = new TeamRepository(dbContext);
        /// var logger = loggerFactory.CreateLogger&lt;TeamService&gt;();
        /// </summary>
        public TeamService(ITeamRepository repo, ILogger<TeamService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        /// <summary>
        /// Get all teams from database
        /// 
        /// Service responsibilities:
        /// 1. Call repository method (data access)
        /// 2. Log the operation (debugging)
        /// 3. Process/validate results if needed
        /// 4. Return to controller
        /// 
        /// Async: Doesn't block thread while database query executes
        /// Thread-safe: Unblock thread to handle other requests
        /// Performance: ~50 threads can handle 1000+ concurrent requests (non-blocking)
        /// </summary>
        public async Task<IEnumerable<Team>> GetAllAsync()
        {
            _logger.LogDebug("Retrieving all teams from database");
            var teams = await _repo.GetAllAsync();  // Execute: SELECT * FROM Teams
            _logger.LogDebug("Retrieved {Count} teams from database", teams.Count());
            return teams;  // Return to controller
        }

        /// <summary>
        /// Get a single team by ID
        /// 
        /// Demonstrates error handling and logging:
        /// - Log at debug level if found (expected case)
        /// - Log at warning level if not found (unexpected, but not critical)
        /// - Return null and let controller return 404 Not Found
        /// </summary>
        public async Task<Team?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Retrieving team with ID {TeamId} from database", id);
            var team = await _repo.GetByIdAsync(id);  // Execute: SELECT * FROM Teams WHERE TeamId = {id}
            if (team == null)
                _logger.LogWarning("Team with ID {TeamId} not found in database", id);
            else
                _logger.LogDebug("Successfully retrieved team with ID {TeamId}", id);
            return team;
        }

        /// <summary>
        /// Create a new team
        /// 
        /// Transaction flow:
        /// 1. Receive team object (from controller, already validated)
        /// 2. Add to repository (stages in EF Core change tracker)
        /// 3. SaveChangesAsync() executes SQL INSERT:
        ///    INSERT INTO Teams (Name, Coach, FoundedYear) VALUES (...)
        /// 4. Database auto-generates TeamId and returns it
        /// 5. Object now has TeamId populated
        /// 
        /// Why async?
        /// SaveChangesAsync() executes SQL to database (slow I/O operation)
        /// Async allows thread to handle other requests while waiting
        /// </summary>
        public async Task<Team> CreateAsync(Team team)
        {
            _logger.LogInformation("Creating new team: {TeamName}", team.Name);
            
            // Stage in EF Core: Add to change tracker, but not saved yet
            await _repo.AddAsync(team);
            
            // Execute: Actually save changes to database
            // Generates: INSERT INTO Teams (Name, Coach, FoundedYear) VALUES (...)
            // Returns: team.TeamId is now populated (auto-generated by database)
            await _repo.SaveChangesAsync();
            
            _logger.LogInformation("Successfully created team with ID {TeamId}", team.TeamId);
            return team;  // Return to controller, which serializes to JSON response
        }

        /// <summary>
        /// Update an existing team
        /// 
        /// Business rule validation:
        /// - URL ID must match object TeamId
        /// - Example: PUT /api/teams/5 with body { TeamId: 5 }
        /// - If URL ID = 10 but body TeamId = 5, that's a conflict!
        /// - Prevents accidentally updating the wrong team
        /// 
        /// Transaction flow:
        /// 1. Validate ID consistency
        /// 2. Update in EF (mark as modified)
        /// 3. SaveChangesAsync executes UPDATE statement
        /// 4. Return success/failure
        /// </summary>
        public async Task<bool> UpdateAsync(int id, Team team)
        {
            _logger.LogInformation("Updating team with ID {TeamId}", id);
            
            // ===== BUSINESS RULE VALIDATION =====
            // Check that URL parameter ID matches the object ID
            // This prevents confusing mistakes like PUT /api/teams/5 with { TeamId: 99 }
            if (id != team.TeamId)
            {
                _logger.LogWarning("Update failed: ID mismatch - URL ID {UrlId} != Team ID {TeamId}", id, team.TeamId);
                // Return false and let controller return 400 Bad Request
                return false;
            }
            
            // Update in EF Core: Mark entity as modified
            await _repo.UpdateAsync(team);
            // Generates: UPDATE Teams SET Name=..., Coach=..., FoundedYear=... WHERE TeamId={id}
            
            // Execute the update
            var success = await _repo.SaveChangesAsync();
            
            if (success)
                _logger.LogInformation("Successfully updated team with ID {TeamId}", id);
            else
                _logger.LogWarning("Failed to update team with ID {TeamId}", id);
                
            return success;
        }

        /// <summary>
        /// Delete a team
        /// 
        /// Transaction flow:
        /// 1. Fetch team by ID (to ensure it exists)
        /// 2. Delete from repository (mark for deletion in EF Core)
        /// 3. SaveChangesAsync executes DELETE statement
        /// 4. Return success (or false if team not found)
        /// 
        /// Database constraint:
        /// DeleteBehavior.Restrict on Match.HomeTeamId and Match.AwayTeamId prevents deletion
        /// If team has matches: DELETE fails with FK constraint violation
        /// Exception is caught by GlobalExceptionHandlingMiddleware → 500 or custom handler
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting team with ID {TeamId}", id);
            
            // Fetch team to ensure it exists before attempting delete
            var team = await _repo.GetByIdAsync(id);
            if (team == null)
            {
                _logger.LogWarning("Delete failed: Team with ID {TeamId} not found", id);
                // Return false and let controller return 404 Not Found
                return false;
            }

            if (await _repo.HasPlayersAsync(id))
            {
                _logger.LogWarning("Delete blocked: Team with ID {TeamId} has assigned players", id);
                throw new InvalidOperationException("Cannot delete team because it still has players assigned. Reassign or delete those players first.");
            }

            if (await _repo.HasMatchesAsync(id))
            {
                _logger.LogWarning("Delete blocked: Team with ID {TeamId} is referenced by matches", id);
                throw new InvalidOperationException("Cannot delete team because it is referenced by existing matches.");
            }
            
            // Delete from EF Core: Mark for deletion
            await _repo.DeleteAsync(team);
            // Executes: DELETE FROM Teams WHERE TeamId={id}
            // NOTE: If team has matches (FK constraint), database will reject this with error
            
            // Execute the deletion
            var success = await _repo.SaveChangesAsync();
            
            if (success)
                _logger.LogInformation("Successfully deleted team with ID {TeamId}", id);
            else
                _logger.LogWarning("Failed to delete team with ID {TeamId}", id);
                
            return success;
        }
    }
}

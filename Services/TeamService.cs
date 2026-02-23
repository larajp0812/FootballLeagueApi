using FootballLeagueApi.Models;
using FootballLeagueApi.Repositories;

namespace FootballLeagueApi.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _repo;
        private readonly ILogger<TeamService> _logger;

        public TeamService(ITeamRepository repo, ILogger<TeamService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<IEnumerable<Team>> GetAllAsync()
        {
            _logger.LogDebug("Retrieving all teams from database");
            var teams = await _repo.GetAllAsync();
            _logger.LogDebug("Retrieved {Count} teams from database", teams.Count());
            return teams;
        }

        public async Task<Team?> GetByIdAsync(int id)
        {
            _logger.LogDebug("Retrieving team with ID {TeamId} from database", id);
            var team = await _repo.GetByIdAsync(id);
            if (team == null)
                _logger.LogWarning("Team with ID {TeamId} not found in database", id);
            else
                _logger.LogDebug("Successfully retrieved team with ID {TeamId}", id);
            return team;
        }

        public async Task<Team> CreateAsync(Team team)
        {
            _logger.LogInformation("Creating new team: {TeamName}", team.Name);
            await _repo.AddAsync(team);
            await _repo.SaveChangesAsync();
            _logger.LogInformation("Successfully created team with ID {TeamId}", team.TeamId);
            return team;
        }

        public async Task<bool> UpdateAsync(int id, Team team)
        {
            _logger.LogInformation("Updating team with ID {TeamId}", id);
            if (id != team.TeamId)
            {
                _logger.LogWarning("Update failed: ID mismatch - URL ID {UrlId} != Team ID {TeamId}", id, team.TeamId);
                return false;
            }
            await _repo.UpdateAsync(team);
            var success = await _repo.SaveChangesAsync();
            if (success)
                _logger.LogInformation("Successfully updated team with ID {TeamId}", id);
            else
                _logger.LogWarning("Failed to update team with ID {TeamId}", id);
            return success;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("Deleting team with ID {TeamId}", id);
            var team = await _repo.GetByIdAsync(id);
            if (team == null)
            {
                _logger.LogWarning("Delete failed: Team with ID {TeamId} not found", id);
                return false;
            }
            await _repo.DeleteAsync(team);
            var success = await _repo.SaveChangesAsync();
            if (success)
                _logger.LogInformation("Successfully deleted team with ID {TeamId}", id);
            else
                _logger.LogWarning("Failed to delete team with ID {TeamId}", id);
            return success;
        }
    }
}

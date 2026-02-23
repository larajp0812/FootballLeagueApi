using FootballLeagueApi.DTOs;
using FootballLeagueApi.Models;
using FootballLeagueApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly ILogger<TeamsController> _logger;

        public TeamsController(ITeamService teamService, ILogger<TeamsController> logger)
        {
            _teamService = teamService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Fetching all teams");
                var teams = await _teamService.GetAllAsync();
                _logger.LogInformation("Successfully retrieved {Count} teams", teams.Count());
                var dto = teams.Select(t => TeamMapper.ToDto(t));
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all teams");
                throw;
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching team with ID {TeamId}", id);
                var team = await _teamService.GetByIdAsync(id);
                if (team == null)
                {
                    _logger.LogWarning("Team with ID {TeamId} not found", id);
                    return NotFound();
                }
                _logger.LogInformation("Successfully retrieved team with ID {TeamId}", id);
                return Ok(TeamMapper.ToDto(team));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching team with ID {TeamId}", id);
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(TeamCreateDto dto)
        {
            try
            {
                _logger.LogInformation("Creating new team: {TeamName}", dto.Name);
                var team = TeamMapper.ToModel(dto);
                var created = await _teamService.CreateAsync(team);
                _logger.LogInformation("Successfully created team with ID {TeamId}", created.TeamId);
                return CreatedAtAction(nameof(GetById), new { id = created.TeamId }, TeamMapper.ToDto(created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating team: {TeamName}", dto.Name);
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TeamUpdateDto dto)
        {
            try
            {
                _logger.LogInformation("Updating team with ID {TeamId}: {TeamName}", id, dto.Name);
                var team = new Team
                {
                    TeamId = id,
                    Name = dto.Name,
                    Coach = dto.Coach,
                    FoundedYear = dto.FoundedYear
                };

                var success = await _teamService.UpdateAsync(id, team);
                if (!success)
                {
                    _logger.LogWarning("Team with ID {TeamId} not found for update", id);
                    return NotFound();
                }

                _logger.LogInformation("Successfully updated team with ID {TeamId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating team with ID {TeamId}", id);
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Deleting team with ID {TeamId}", id);
                var success = await _teamService.DeleteAsync(id);
                if (!success)
                {
                    _logger.LogWarning("Team with ID {TeamId} not found for deletion", id);
                    return NotFound();
                }

                _logger.LogInformation("Successfully deleted team with ID {TeamId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting team with ID {TeamId}", id);
                throw;
            }
        }
    }
}

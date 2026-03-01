using FootballLeagueApi.DTOs;
using FootballLeagueApi.Models;
using FootballLeagueApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueApi.Controllers
{
    /// <summary>
    /// Matches Controller - Manages REST API operations for league matches
    /// 
    /// A match represents a game between two teams (home and away) played at a specific venue during a season.
    /// Each match has a scheduled kickoff time and can optionally have results (goals, attendance).
    /// Matches are associated with match events (goals, red cards, substitutions, etc.) that track specific occurrences during play.
    /// 
    /// This controller manages match scheduling, result recording, and deletion.
    /// Base route: /api/matches
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchService _matchService;
        private readonly ILogger<MatchesController> _logger;

        /// <summary>
        /// Constructor for dependency injection of MatchService and Logger
        /// </summary>
        /// <param name="matchService">Service for match business logic operations</param>
        /// <param name="logger">Logger for tracking operations</param>
        public MatchesController(IMatchService matchService, ILogger<MatchesController> logger)
        {
            _matchService = matchService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieve all matches
        /// </summary>
        /// <returns>Array of MatchReadDto objects</returns>
        /// <response code="200">List retrieved successfully</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MatchReadDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogDebug("Fetching all matches");
            var matches = await _matchService.GetAllAsync();
            var dtos = matches.Select(m => MatchMapper.ToDto(m));
            return Ok(dtos);
        }

        /// <summary>
        /// Retrieve a specific match by ID
        /// </summary>
        /// <param name="id">Match ID</param>
        /// <returns>MatchReadDto with match details including teams, venue, and kickoff time</returns>
        /// <response code="200">Match found</response>
        /// <response code="404">Match not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MatchReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogDebug("Fetching match with ID {MatchId}", id);
            var match = await _matchService.GetByIdAsync(id);
            if (match == null)
            {
                _logger.LogWarning("Match with ID {MatchId} not found", id);
                return NotFound();
            }

            return Ok(MatchMapper.ToDto(match));
        }

        /// <summary>
        /// Create a new match
        /// </summary>
        /// <param name="dto">MatchCreateDto with match details (home team, away team, season, venue, kickoff time)</param>
        /// <returns>Created MatchReadDto with assigned ID</returns>
        /// <response code="201">Match created successfully</response>
        /// <response code="400">Invalid match data or business rule violation</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(MatchReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(MatchCreateDto dto)
        {
            try
            {
                _logger.LogInformation("Creating new match between team {HomeTeamId} and team {AwayTeamId}", 
                    dto.HomeTeamId, dto.AwayTeamId);
                var match = MatchMapper.ToModel(dto);
                var created = await _matchService.CreateAsync(match);
                _logger.LogInformation("Successfully created match with ID {MatchId}", created.MatchId);
                return CreatedAtAction(nameof(GetById), new { id = created.MatchId }, MatchMapper.ToDto(created));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating match: {ErrorMessage}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing match's information
        /// Typically used to record the final score and match result after play completes
        /// </summary>
        /// <param name="id">Match ID to update</param>
        /// <param name="dto">MatchUpdateDto with updated match information (scores, attendance)</param>
        /// <returns>204 No Content</returns>
        /// <response code="204">Match updated successfully</response>
        /// <response code="404">Match not found</response>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, MatchUpdateDto dto)
        {
            _logger.LogInformation("Updating match with ID {MatchId}", id);
            var match = await _matchService.GetByIdAsync(id);
            if (match == null)
            {
                _logger.LogWarning("Match with ID {MatchId} not found for update", id);
                return NotFound();
            }
            var updated = MatchMapper.ToModel(dto, id, match.HomeTeamId, match.AwayTeamId, match.SeasonId, match.VenueId);
            var success = await _matchService.UpdateAsync(id, updated);
            if (!success)
            {
                _logger.LogWarning("Failed to update match with ID {MatchId}", id);
                return BadRequest();
            }

            _logger.LogInformation("Successfully updated match with ID {MatchId}", id);
            return NoContent();
        }

        /// <summary>
        /// Delete a match
        /// Warning: Deleting a match will remove all associated match events
        /// </summary>
        /// <param name="id">Match ID to delete</param>
        /// <returns>204 No Content</returns>
        /// <response code="204">Match deleted successfully</response>
        /// <response code="404">Match not found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting match with ID {MatchId}", id);
            var success = await _matchService.DeleteAsync(id);
            if (!success)
            {
                _logger.LogWarning("Match with ID {MatchId} not found for deletion", id);
                return NotFound();
            }

            _logger.LogInformation("Successfully deleted match with ID {MatchId}", id);
            return NoContent();
        }
    }
}

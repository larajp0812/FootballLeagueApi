using FootballLeagueApi.DTOs;
using FootballLeagueApi.Models;
using FootballLeagueApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueApi.Controllers
{
    /// <summary>
    /// Matches Controller - Handles HTTP requests for match operations
    /// This controller provides REST API endpoints for managing football matches.
    /// Matches represent games between teams with results and associated events.
    /// Maps to /api/matches endpoint.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        /// <summary>
        /// Dependency-injected MatchService instance
        /// Contains business logic for match operations
        /// </summary>
        private readonly IMatchService _matchService;

        /// <summary>
        /// Constructor accepting IMatchService through dependency injection
        /// </summary>
        public MatchesController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        /// <summary>
        /// GET /api/matches - Retrieve all matches
        /// Returns 200 OK with a list of all scheduled and completed matches
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var matches = await _matchService.GetAllAsync();
            var dtos = matches.Select(m => MatchMapper.ToDto(m));
            return Ok(dtos);
        }

        /// <summary>
        /// GET /api/matches/{id} - Retrieve a specific match by ID
        /// Returns 200 OK if found, 404 Not Found if not found
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var match = await _matchService.GetByIdAsync(id);
            if (match == null)
                return NotFound();

            return Ok(MatchMapper.ToDto(match));
        }

        /// <summary>
        /// POST /api/matches - Create a new match
        /// Accepts a Match object with HomeTeamId, AwayTeamId, SeasonId, VenueId, and KickoffTime
        /// Includes error handling for business logic violations
        /// Returns 201 Created with the newly created match data
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(MatchCreateDto dto)
        {
            try
            {
                var match = MatchMapper.ToModel(dto);
                var created = await _matchService.CreateAsync(match);
                return CreatedAtAction(nameof(GetById), new { id = created.MatchId }, MatchMapper.ToDto(created));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// PUT /api/matches/{id} - Update an existing match
        /// Typically used to record the final score and match result
        /// Accepts updated Match object in the request body
        /// Returns 204 No Content on success, 400 Bad Request on failure
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MatchUpdateDto dto)
        {
            var match = await _matchService.GetByIdAsync(id);
            if (match == null)
                return NotFound();
            var updated = MatchMapper.ToModel(dto, id, match.HomeTeamId, match.AwayTeamId, match.SeasonId, match.VenueId);
            var success = await _matchService.UpdateAsync(id, updated);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        /// <summary>
        /// DELETE /api/matches/{id} - Delete a match
        /// Returns 204 No Content on success, 404 Not Found if match doesn't exist
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _matchService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}

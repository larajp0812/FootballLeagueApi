using FootballLeagueApi.DTOs;
using FootballLeagueApi.Models;
using FootballLeagueApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueApi.Controllers
{
    /// <summary>
    /// Seasons Controller - Manages REST API operations for league seasons
    /// 
    /// A season represents a complete period (e.g., 2025/26) during which all league matches are played.
    /// This controller manages season creation, retrieval, updates, and deletion.
    /// Multiple matches are associated with each season, and players participate in seasons through their teams.
    /// 
    /// Base route: /api/seasons
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SeasonsController : ControllerBase
    {
        private readonly ISeasonService _seasonService;
        private readonly ILogger<SeasonsController> _logger;

        /// <summary>
        /// Constructor for dependency injection of SeasonService and Logger
        /// </summary>
        /// <param name="seasonService">Service for season business logic operations</param>
        /// <param name="logger">Logger for tracking operations</param>
        public SeasonsController(ISeasonService seasonService, ILogger<SeasonsController> logger)
        {
            _seasonService = seasonService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieve all seasons
        /// </summary>
        /// <returns>Array of SeasonReadDto objects</returns>
        /// <response code="200">List retrieved successfully</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SeasonReadDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogDebug("Fetching all seasons");
            var seasons = await _seasonService.GetAllAsync();
            var dtos = seasons.Select(s => SeasonMapper.ToDto(s));
            return Ok(dtos);
        }

        /// <summary>
        /// Retrieve a specific season by ID
        /// </summary>
        /// <param name="id">Season ID</param>
        /// <returns>SeasonReadDto with season details</returns>
        /// <response code="200">Season found</response>
        /// <response code="404">Season not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SeasonReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogDebug("Fetching season with ID {SeasonId}", id);
            var season = await _seasonService.GetByIdAsync(id);
            if (season == null)
            {
                _logger.LogWarning("Season with ID {SeasonId} not found", id);
                return NotFound();
            }
            return Ok(SeasonMapper.ToDto(season));
        }

        /// <summary>
        /// Create a new season
        /// </summary>
        /// <param name="dto">SeasonCreateDto with season information</param>
        /// <returns>Created SeasonReadDto with assigned ID</returns>
        /// <response code="201">Season created successfully</response>
        /// <response code="400">Invalid season data</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(SeasonReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(SeasonCreateDto dto)
        {
            _logger.LogInformation("Creating new season: {SeasonName}", dto.Name);
            var season = SeasonMapper.ToModel(dto);
            var created = await _seasonService.CreateAsync(season);
            _logger.LogInformation("Successfully created season with ID {SeasonId}", created.SeasonId);
            return CreatedAtAction(nameof(GetById), new { id = created.SeasonId }, SeasonMapper.ToDto(created));
        }

        /// <summary>
        /// Update an existing season's information
        /// </summary>
        /// <param name="id">Season ID to update</param>
        /// <param name="dto">SeasonUpdateDto with updated season information</param>
        /// <returns>204 No Content</returns>
        /// <response code="204">Season updated successfully</response>
        /// <response code="404">Season not found</response>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, SeasonUpdateDto dto)
        {
            _logger.LogInformation("Updating season with ID {SeasonId}", id);
            var season = SeasonMapper.ToModel(dto, id);
            var success = await _seasonService.UpdateAsync(id, season);
            if (!success)
            {
                _logger.LogWarning("Season with ID {SeasonId} not found for update", id);
                return BadRequest();
            }
            _logger.LogInformation("Successfully updated season with ID {SeasonId}", id);
            return NoContent();
        }

        /// <summary>
        /// Delete a season
        /// </summary>
        /// <param name="id">Season ID to delete</param>
        /// <returns>204 No Content</returns>
        /// <response code="204">Season deleted successfully</response>
        /// <response code="404">Season not found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting season with ID {SeasonId}", id);
            var success = await _seasonService.DeleteAsync(id);
            if (!success)
            {
                _logger.LogWarning("Season with ID {SeasonId} not found for deletion", id);
                return NotFound();
            }
            _logger.LogInformation("Successfully deleted season with ID {SeasonId}", id);
            return NoContent();
        }
    }
}

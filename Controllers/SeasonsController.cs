using FootballLeagueApi.DTOs;
using FootballLeagueApi.Models;
using FootballLeagueApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueApi.Controllers
{
    /// <summary>
    /// Seasons Controller - Handles HTTP requests for season operations
    /// A season is a time period during which all league matches are played (e.g., 2025/26).
    /// Maps to /api/seasons endpoint.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SeasonsController : ControllerBase
    {
        /// <summary>
        /// Dependency-injected SeasonService instance
        /// Contains business logic for season operations
        /// </summary>
        private readonly ISeasonService _seasonService;

        /// <summary>
        /// Constructor accepting ISeasonService through dependency injection
        /// </summary>
        public SeasonsController(ISeasonService seasonService)
        {
            _seasonService = seasonService;
        }

        /// <summary>
        /// GET /api/seasons - Retrieve all seasons
        /// Returns 200 OK with a list of all seasons
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var seasons = await _seasonService.GetAllAsync();
            var dtos = seasons.Select(s => SeasonMapper.ToDto(s));
            return Ok(dtos);
        }

        /// <summary>
        /// GET /api/seasons/{id} - Retrieve a specific season by ID
        /// Returns 200 OK if found, 404 Not Found if not found
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var season = await _seasonService.GetByIdAsync(id);
            if (season == null)
                return NotFound();

            return Ok(SeasonMapper.ToDto(season));
        }

        /// <summary>
        /// POST /api/seasons - Create a new season
        /// Accepts a Season object with Name, StartDate, and EndDate
        /// Returns 201 Created with the newly created season data
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(SeasonCreateDto dto)
        {
            var season = SeasonMapper.ToModel(dto);
            var created = await _seasonService.CreateAsync(season);
            return CreatedAtAction(nameof(GetById), new { id = created.SeasonId }, SeasonMapper.ToDto(created));
        }

        /// <summary>
        /// PUT /api/seasons/{id} - Update an existing season
        /// Accepts updated Season object in the request body
        /// Returns 204 No Content on success, 400 Bad Request on failure
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SeasonUpdateDto dto)
        {
            var updated = SeasonMapper.ToModel(dto, id);
            var success = await _seasonService.UpdateAsync(id, updated);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        /// <summary>
        /// DELETE /api/seasons/{id} - Delete a season
        /// Returns 204 No Content on success, 404 Not Found if season doesn't exist
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _seasonService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}

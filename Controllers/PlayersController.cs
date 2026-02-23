using FootballLeagueApi.DTOs;
using FootballLeagueApi.Models;
using FootballLeagueApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueApi.Controllers
{
    /// <summary>
    /// Players Controller - Handles HTTP requests for player operations
    /// This controller provides REST API endpoints for managing players.
    /// Maps to /api/players endpoint.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        /// <summary>
        /// Dependency-injected PlayerService instance
        /// Contains business logic for player operations
        /// </summary>
        private readonly IPlayerService _playerService;

        /// <summary>
        /// Constructor accepting IPlayerService through dependency injection
        /// </summary>
        public PlayersController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        /// <summary>
        /// GET /api/players - Retrieve all players
        /// Returns 200 OK with a list of all players in the system
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var players = await _playerService.GetAllAsync();
            var dtos = players.Select(p => PlayerMapper.ToDto(p));
            return Ok(dtos);
        }

        /// <summary>
        /// GET /api/players/{id} - Retrieve a specific player by ID
        /// Returns 200 OK if found, 404 Not Found if not found
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var player = await _playerService.GetByIdAsync(id);
            if (player == null)
                return NotFound();

            return Ok(PlayerMapper.ToDto(player));
        }

        /// <summary>
        /// POST /api/players - Create a new player
        /// Accepts a Player object in the request body
        /// Returns 201 Created with the newly created player data
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(PlayerCreateDto dto)
        {
            var player = PlayerMapper.ToModel(dto);
            var created = await _playerService.CreateAsync(player);
            return CreatedAtAction(nameof(GetById), new { id = created.PlayerId }, PlayerMapper.ToDto(created));
        }

        /// <summary>
        /// PUT /api/players/{id} - Update an existing player
        /// Accepts updated Player object in the request body
        /// Returns 204 No Content on success, 400 Bad Request on failure
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PlayerUpdateDto dto)
        {
            var player = await _playerService.GetByIdAsync(id);
            if (player == null)
                return NotFound();
            var updated = PlayerMapper.ToModel(dto, id, player.TeamId);
            var success = await _playerService.UpdateAsync(id, updated);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        /// <summary>
        /// DELETE /api/players/{id} - Delete a player
        /// Returns 204 No Content on success, 404 Not Found if player doesn't exist
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _playerService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}

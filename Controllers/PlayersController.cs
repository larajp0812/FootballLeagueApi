using FootballLeagueApi.DTOs;
using FootballLeagueApi.Models;
using FootballLeagueApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueApi.Controllers
{
    /// <summary>
    /// Players Controller - Manages REST API operations for football players
    /// 
    /// This controller provides endpoints for managing individual player records including creation, 
    /// retrieval, updates, and deletion. Players are associated with teams and can be tracked in matches.
    /// Each player has a unique ID, full name, shirt number, position, and team assignment.
    /// 
    /// Base route: /api/players
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly ILogger<PlayersController> _logger;

        /// <summary>
        /// Constructor for dependency injection of PlayerService and Logger
        /// </summary>
        /// <param name="playerService">Service for player business logic operations</param>
        /// <param name="logger">Logger for tracking operations and errors</param>
        public PlayersController(IPlayerService playerService, ILogger<PlayersController> logger)
        {
            _playerService = playerService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieve all players in the league
        /// </summary>
        /// <returns>Array of PlayerReadDto objects</returns>
        /// <response code="200">List retrieved successfully</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PlayerReadDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogDebug("Fetching all players");
            var players = await _playerService.GetAllAsync();
            var dtos = players.Select(p => PlayerMapper.ToDto(p));
            return Ok(dtos);
        }

        /// <summary>
        /// Retrieve a specific player by ID
        /// </summary>
        /// <param name="id">Player ID</param>
        /// <returns>PlayerReadDto with player details</returns>
        /// <response code="200">Player found</response>
        /// <response code="404">Player not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PlayerReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogDebug("Fetching player with ID {PlayerId}", id);
            var player = await _playerService.GetByIdAsync(id);
            if (player == null)
            {
                _logger.LogWarning("Player with ID {PlayerId} not found", id);
                return NotFound();
            }
            return Ok(PlayerMapper.ToDto(player));
        }

        /// <summary>
        /// Create a new player
        /// </summary>
        /// <param name="dto">PlayerCreateDto with player information</param>
        /// <returns>Created PlayerReadDto with assigned ID</returns>
        /// <response code="201">Player created successfully</response>
        /// <response code="400">Invalid player data</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(PlayerReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(PlayerCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Create player failed: Invalid model state");
                return BadRequest(ModelState);
            }
            _logger.LogInformation("Creating new player: {PlayerName}", dto.FullName);
            var player = PlayerMapper.ToModel(dto);
            var created = await _playerService.CreateAsync(player);
            _logger.LogInformation("Successfully created player with ID {PlayerId}", created.PlayerId);
            return CreatedAtAction(nameof(GetById), new { id = created.PlayerId }, PlayerMapper.ToDto(created));
        }

        /// <summary>
        /// Update an existing player's information
        /// </summary>
        /// <param name="id">Player ID to update</param>
        /// <param name="dto">PlayerUpdateDto with updated player information</param>
        /// <returns>204 No Content</returns>
        /// <response code="204">Player updated successfully</response>
        /// <response code="404">Player not found</response>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, PlayerUpdateDto dto)
        {
            _logger.LogInformation("Updating player with ID {PlayerId}", id);
            var existingPlayer = await _playerService.GetByIdAsync(id);
            if (existingPlayer == null)
            {
                _logger.LogWarning("Player with ID {PlayerId} not found for update", id);
                return NotFound();
            }
            var player = PlayerMapper.ToModel(dto, id, existingPlayer.TeamId);
            var success = await _playerService.UpdateAsync(id, player);
            if (!success)
            {
                _logger.LogWarning("Player with ID {PlayerId} not found for update", id);
                return NotFound();
            }
            _logger.LogInformation("Successfully updated player with ID {PlayerId}", id);
            return NoContent();
        }

        /// <summary>
        /// Delete a player
        /// </summary>
        /// <param name="id">Player ID to delete</param>
        /// <returns>204 No Content</returns>
        /// <response code="204">Player deleted successfully</response>
        /// <response code="404">Player not found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting player with ID {PlayerId}", id);
            var success = await _playerService.DeleteAsync(id);
            if (!success)
            {
                _logger.LogWarning("Player with ID {PlayerId} not found for deletion", id);
                return NotFound();
            }
            _logger.LogInformation("Successfully deleted player with ID {PlayerId}", id);
            return NoContent();
        }
    }
}

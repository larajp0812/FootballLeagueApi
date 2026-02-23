using FootballLeagueApi.DTOs;
using FootballLeagueApi.Models;
using FootballLeagueApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueApi.Controllers
{
    /// <summary>
    /// Match Events Controller - Manages REST API operations for match events
    /// 
    /// Match Events capture specific occurrences during a match (goals, yellow cards, red cards, substitutions, etc.).
    /// Events are associated with a specific match and typically involve a player (optional for non-player events).
    /// Each event has a minute timestamp indicating when it occurred during play.
    /// Events are primarily recorded through player actions but support tracking of major match incidents.
    /// 
    /// This controller manages event recording, retrieval, updates, and deletion.
    /// Base route: /api/matchevents
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MatchEventsController : ControllerBase
    {
        private readonly IMatchEventService _eventService;
        private readonly ILogger<MatchEventsController> _logger;

        /// <summary>
        /// Constructor for dependency injection of MatchEventService and Logger
        /// </summary>
        /// <param name="eventService">Service for match event business logic operations</param>
        /// <param name="logger">Logger for tracking operations</param>
        public MatchEventsController(IMatchEventService eventService, ILogger<MatchEventsController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieve all match events
        /// </summary>
        /// <returns>Array of MatchEventReadDto objects</returns>
        /// <response code="200">List retrieved successfully</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MatchEventReadDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogDebug("Fetching all match events");
            var eventsList = await _eventService.GetAllAsync();
            var dtos = eventsList.Select(e => MatchEventMapper.ToDto(e));
            return Ok(dtos);
        }

        /// <summary>
        /// Retrieve a specific match event by ID
        /// </summary>
        /// <param name="id">Match Event ID</param>
        /// <returns>MatchEventReadDto with event details including match, player, minute, and type</returns>
        /// <response code="200">Match event found</response>
        /// <response code="404">Match event not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MatchEventReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogDebug("Fetching match event with ID {EventId}", id);
            var matchEvent = await _eventService.GetByIdAsync(id);
            if (matchEvent == null)
            {
                _logger.LogWarning("Match event with ID {EventId} not found", id);
                return NotFound();
            }

            return Ok(MatchEventMapper.ToDto(matchEvent));
        }

        /// <summary>
        /// Create a new match event
        /// Records an event occurrence during a match (goal, card, substitution, etc.)
        /// </summary>
        /// <param name="dto">MatchEventCreateDto with event details (match, player, minute, event type)</param>
        /// <returns>Created MatchEventReadDto with assigned ID</returns>
        /// <response code="201">Match event created successfully</response>
        /// <response code="400">Invalid event data or business rule violation</response>
        [HttpPost]
        [ProducesResponseType(typeof(MatchEventReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(MatchEventCreateDto dto)
        {
            try
            {
                _logger.LogInformation("Creating new match event for match {MatchId} at minute {Minute}", 
                    dto.MatchId, dto.Minute);
                var matchEvent = MatchEventMapper.ToModel(dto);
                var created = await _eventService.CreateAsync(matchEvent);
                _logger.LogInformation("Successfully created match event with ID {EventId}", created.MatchEventId);
                return CreatedAtAction(nameof(GetById), new { id = created.MatchEventId }, MatchEventMapper.ToDto(created));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating match event: {ErrorMessage}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing match event
        /// Used to correct event details (player involved, minute, or event type)
        /// </summary>
        /// <param name="id">Match Event ID to update</param>
        /// <param name="dto">MatchEventUpdateDto with updated event information</param>
        /// <returns>204 No Content</returns>
        /// <response code="204">Match event updated successfully</response>
        /// <response code="404">Match event not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, MatchEventUpdateDto dto)
        {
            _logger.LogInformation("Updating match event with ID {EventId}", id);
            var matchEvent = await _eventService.GetByIdAsync(id);
            if (matchEvent == null)
            {
                _logger.LogWarning("Match event with ID {EventId} not found for update", id);
                return NotFound();
            }
            var updated = MatchEventMapper.ToModel(dto, id, matchEvent.MatchId);
            var success = await _eventService.UpdateAsync(id, updated);
            if (!success)
            {
                _logger.LogWarning("Failed to update match event with ID {EventId}", id);
                return BadRequest();
            }

            _logger.LogInformation("Successfully updated match event with ID {EventId}", id);
            return NoContent();
        }

        /// <summary>
        /// Delete a match event
        /// Removes a recorded event from a match
        /// </summary>
        /// <param name="id">Match Event ID to delete</param>
        /// <returns>204 No Content</returns>
        /// <response code="204">Match event deleted successfully</response>
        /// <response code="404">Match event not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting match event with ID {EventId}", id);
            var success = await _eventService.DeleteAsync(id);
            if (!success)
            {
                _logger.LogWarning("Match event with ID {EventId} not found for deletion", id);
                return NotFound();
            }

            _logger.LogInformation("Successfully deleted match event with ID {EventId}", id);
            return NoContent();
        }
    }
}

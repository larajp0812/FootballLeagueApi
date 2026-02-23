using FootballLeagueApi.DTOs;
using FootballLeagueApi.Models;
using FootballLeagueApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueApi.Controllers
{
    /// <summary>
    /// Match Events Controller - Handles HTTP requests for match event operations
    /// This controller provides REST API endpoints for managing events that occur during matches.
    /// Events include goals, cards (yellow/red), substitutions, and other match incidents.
    /// Maps to /api/matchevents endpoint.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MatchEventsController : ControllerBase
    {
        /// <summary>
        /// Dependency-injected MatchEventService instance
        /// Contains business logic for match event operations
        /// </summary>
        private readonly IMatchEventService _eventService;

        /// <summary>
        /// Constructor accepting IMatchEventService through dependency injection
        /// </summary>
        public MatchEventsController(IMatchEventService eventService)
        {
            _eventService = eventService;
        }

        /// <summary>
        /// GET /api/matchevents - Retrieve all match events
        /// Returns 200 OK with a list of all events that have occurred in matches
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var eventsList = await _eventService.GetAllAsync();
            var dtos = eventsList.Select(e => MatchEventMapper.ToDto(e));
            return Ok(dtos);
        }

        /// <summary>
        /// GET /api/matchevents/{id} - Retrieve a specific match event by ID
        /// Returns 200 OK if found, 404 Not Found if not found
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var matchEvent = await _eventService.GetByIdAsync(id);
            if (matchEvent == null)
                return NotFound();

            return Ok(MatchEventMapper.ToDto(matchEvent));
        }

        /// <summary>
        /// POST /api/matchevents - Create a new match event
        /// Accepts a MatchEvent object with MatchId, PlayerId (optional), Minute, and EventType
        /// Includes error handling for business logic violations
        /// Returns 201 Created with the newly created event data
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(MatchEventCreateDto dto)
        {
            try
            {
                var matchEvent = MatchEventMapper.ToModel(dto);
                var created = await _eventService.CreateAsync(matchEvent);
                return CreatedAtAction(nameof(GetById), new { id = created.MatchEventId }, MatchEventMapper.ToDto(created));
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// PUT /api/matchevents/{id} - Update an existing match event
        /// Accepts updated MatchEvent object in the request body
        /// Returns 204 No Content on success, 400 Bad Request on failure
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MatchEventUpdateDto dto)
        {
            var matchEvent = await _eventService.GetByIdAsync(id);
            if (matchEvent == null)
                return NotFound();
            var updated = MatchEventMapper.ToModel(dto, id, matchEvent.MatchId);
            var success = await _eventService.UpdateAsync(id, updated);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        /// <summary>
        /// DELETE /api/matchevents/{id} - Delete a match event
        /// Returns 204 No Content on success, 404 Not Found if event doesn't exist
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _eventService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}

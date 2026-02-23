using FootballLeagueApi.DTOs;
using FootballLeagueApi.Models;
using FootballLeagueApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueApi.Controllers
{
    /// <summary>
    /// Venues Controller - Manages REST API operations for football stadiums and venues
    /// 
    /// This controller provides endpoints for managing physical locations where matches are played.
    /// Venues contain information about stadium name, location, and capacity.
    /// Each venue can host multiple matches throughout different seasons.
    /// 
    /// Base route: /api/venues
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class VenuesController : ControllerBase
    {
        private readonly IVenueService _venueService;
        private readonly ILogger<VenuesController> _logger;

        /// <summary>
        /// Constructor for dependency injection of VenueService and Logger
        /// </summary>
        /// <param name="venueService">Service for venue business logic operations</param>
        /// <param name="logger">Logger for tracking operations</param>
        public VenuesController(IVenueService venueService, ILogger<VenuesController> logger)
        {
            _venueService = venueService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieve all venues/stadiums
        /// </summary>
        /// <returns>Array of VenueReadDto objects</returns>
        /// <response code="200">List retrieved successfully</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VenueReadDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogDebug("Fetching all venues");
            var venues = await _venueService.GetAllAsync();
            var dtos = venues.Select(v => VenueMapper.ToDto(v));
            return Ok(dtos);
        }

        /// <summary>
        /// Retrieve a specific venue by ID
        /// </summary>
        /// <param name="id">Venue ID</param>
        /// <returns>VenueReadDto with venue details</returns>
        /// <response code="200">Venue found</response>
        /// <response code="404">Venue not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(VenueReadDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogDebug("Fetching venue with ID {VenueId}", id);
            var venue = await _venueService.GetByIdAsync(id);
            if (venue == null)
            {
                _logger.LogWarning("Venue with ID {VenueId} not found", id);
                return NotFound();
            }
            return Ok(VenueMapper.ToDto(venue));
        }

        /// <summary>
        /// Create a new venue
        /// </summary>
        /// <param name="dto">VenueCreateDto with venue information</param>
        /// <returns>Created VenueReadDto with assigned ID</returns>
        /// <response code="201">Venue created successfully</response>
        /// <response code="400">Invalid venue data</response>
        [HttpPost]
        [ProducesResponseType(typeof(VenueReadDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(VenueCreateDto dto)
        {
            _logger.LogInformation("Creating new venue: {VenueName}", dto.Name);
            var venue = VenueMapper.ToModel(dto);
            var created = await _venueService.CreateAsync(venue);
            _logger.LogInformation("Successfully created venue with ID {VenueId}", created.VenueId);
            return CreatedAtAction(nameof(GetById), new { id = created.VenueId }, VenueMapper.ToDto(created));
        }

        /// <summary>
        /// Update an existing venue's information
        /// </summary>
        /// <param name="id">Venue ID to update</param>
        /// <param name="dto">VenueUpdateDto with updated venue information</param>
        /// <returns>204 No Content</returns>
        /// <response code="204">Venue updated successfully</response>
        /// <response code="404">Venue not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, VenueUpdateDto dto)
        {
            _logger.LogInformation("Updating venue with ID {VenueId}", id);
            var venue = VenueMapper.ToModel(dto, id);
            var success = await _venueService.UpdateAsync(id, venue);
            if (!success)
            {
                _logger.LogWarning("Venue with ID {VenueId} not found for update", id);
                return BadRequest();
            }
            _logger.LogInformation("Successfully updated venue with ID {VenueId}", id);
            return NoContent();
        }

        /// <summary>
        /// Delete a venue
        /// </summary>
        /// <param name="id">Venue ID to delete</param>
        /// <returns>204 No Content</returns>
        /// <response code="204">Venue deleted successfully</response>
        /// <response code="404">Venue not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting venue with ID {VenueId}", id);
            var success = await _venueService.DeleteAsync(id);
            if (!success)
            {
                _logger.LogWarning("Venue with ID {VenueId} not found for deletion", id);
                return NotFound();
            }
            _logger.LogInformation("Successfully deleted venue with ID {VenueId}", id);
            return NoContent();
        }
    }
}

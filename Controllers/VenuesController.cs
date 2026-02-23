using FootballLeagueApi.DTOs;
using FootballLeagueApi.Models;
using FootballLeagueApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueApi.Controllers
{
    /// <summary>
    /// Venues Controller - Handles HTTP requests for venue operations
    /// This controller provides REST API endpoints for managing football stadiums and venues.
    /// Venues are the physical locations where matches are played.
    /// Maps to /api/venues endpoint.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class VenuesController : ControllerBase
    {
        /// <summary>
        /// Dependency-injected VenueService instance
        /// Contains business logic for venue operations
        /// </summary>
        private readonly IVenueService _venueService;

        /// <summary>
        /// Constructor accepting IVenueService through dependency injection
        /// </summary>
        public VenuesController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        /// <summary>
        /// GET /api/venues - Retrieve all venues
        /// Returns 200 OK with a list of all stadiums/venues
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var venues = await _venueService.GetAllAsync();
            var dtos = venues.Select(v => VenueMapper.ToDto(v));
            return Ok(dtos);
        }

        /// <summary>
        /// GET /api/venues/{id} - Retrieve a specific venue by ID
        /// Returns 200 OK if found, 404 Not Found if not found
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var venue = await _venueService.GetByIdAsync(id);
            if (venue == null)
                return NotFound();

            return Ok(VenueMapper.ToDto(venue));
        }

        /// <summary>
        /// POST /api/venues - Create a new venue
        /// Accepts a Venue object with Name and optional Address
        /// Returns 201 Created with the newly created venue data
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(VenueCreateDto dto)
        {
            var venue = VenueMapper.ToModel(dto);
            var created = await _venueService.CreateAsync(venue);
            return CreatedAtAction(nameof(GetById), new { id = created.VenueId }, VenueMapper.ToDto(created));
        }

        /// <summary>
        /// PUT /api/venues/{id} - Update an existing venue
        /// Accepts updated Venue object in the request body
        /// Returns 204 No Content on success, 400 Bad Request on failure
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, VenueUpdateDto dto)
        {
            var updated = VenueMapper.ToModel(dto, id);
            var success = await _venueService.UpdateAsync(id, updated);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        /// <summary>
        /// DELETE /api/venues/{id} - Delete a venue
        /// Returns 204 No Content on success, 404 Not Found if venue doesn't exist
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _venueService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}

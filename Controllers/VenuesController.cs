using FootballLeagueApi.Models;
using FootballLeagueApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenuesController : ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenuesController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var venues = await _venueService.GetAllAsync();
            return Ok(venues);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var venue = await _venueService.GetByIdAsync(id);
            if (venue == null)
                return NotFound();

            return Ok(venue);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Venue venue)
        {
            var created = await _venueService.CreateAsync(venue);
            return CreatedAtAction(nameof(GetById), new { id = created.VenueId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Venue venue)
        {
            var success = await _venueService.UpdateAsync(id, venue);
            if (!success)
                return BadRequest();

            return NoContent();
        }

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

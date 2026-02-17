using FootballLeagueApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VenuesController : ControllerBase
    {
        private readonly LeagueContext _context;

        public VenuesController(LeagueContext context)
        {
            _context = context;
        }

        // GET: api/venues
        [HttpGet]
        public async Task<IActionResult> GetVenues()
        {
            var venues = await _context.Venues.ToListAsync();
            return Ok(venues);
        }

        // GET: api/venues/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVenue(int id)
        {
            var venue = await _context.Venues.FindAsync(id);

            if (venue == null)
                return NotFound();

            return Ok(venue);
        }

        // POST: api/venues
        [HttpPost]
        public async Task<IActionResult> CreateVenue(Venue venue)
        {
            _context.Venues.Add(venue);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVenue), new { id = venue.VenueId }, venue);
        }

        // PUT: api/venues/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVenue(int id, Venue updatedVenue)
        {
            if (id != updatedVenue.VenueId)
                return BadRequest("Venue ID mismatch");

            _context.Entry(updatedVenue).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Venues.Any(v => v.VenueId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/venues/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenue(int id)
        {
            var venue = await _context.Venues.FindAsync(id);

            if (venue == null)
                return NotFound();

            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

using FootballLeagueApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchEventsController : ControllerBase
    {
        private readonly LeagueContext _context;

        public MatchEventsController(LeagueContext context)
        {
            _context = context;
        }

        // GET: api/matchevents
        [HttpGet]
        public async Task<IActionResult> GetMatchEvents()
        {
            var eventsList = await _context.MatchEvents
                .Include(e => e.Match)
                .Include(e => e.Player)
                .ToListAsync();

            return Ok(eventsList);
        }

        // GET: api/matchevents/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMatchEvent(int id)
        {
            var matchEvent = await _context.MatchEvents
                .Include(e => e.Match)
                .Include(e => e.Player)
                .FirstOrDefaultAsync(e => e.MatchEventId == id);

            if (matchEvent == null)
                return NotFound();

            return Ok(matchEvent);
        }

        // POST: api/matchevents
        [HttpPost]
        public async Task<IActionResult> CreateMatchEvent(MatchEvent matchEvent)
        {
            _context.MatchEvents.Add(matchEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMatchEvent), new { id = matchEvent.MatchEventId }, matchEvent);
        }

        // PUT: api/matchevents/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMatchEvent(int id, MatchEvent updatedEvent)
        {
            if (id != updatedEvent.MatchEventId)
                return BadRequest("MatchEvent ID mismatch");

            _context.Entry(updatedEvent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.MatchEvents.Any(e => e.MatchEventId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/matchevents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatchEvent(int id)
        {
            var matchEvent = await _context.MatchEvents.FindAsync(id);

            if (matchEvent == null)
                return NotFound();

            _context.MatchEvents.Remove(matchEvent);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

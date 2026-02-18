using FootballLeagueApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeasonsController : ControllerBase
    {
        private readonly LeagueContext _context;

        public SeasonsController(LeagueContext context)
        {
            _context = context;
        }

        // GET: api/seasons
        [HttpGet]
        public async Task<IActionResult> GetSeasons()
        {
            var seasons = await _context.Seasons.ToListAsync();
            return Ok(seasons);
        }

        // GET: api/seasons/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSeason(int id)
        {
            var season = await _context.Seasons.FindAsync(id);

            if (season == null)
                return NotFound();

            return Ok(season);
        }

        // POST: api/seasons
        [HttpPost]
        public async Task<IActionResult> CreateSeason(Season season)
        {
            _context.Seasons.Add(season);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSeason), new { id = season.SeasonId }, season);
        }

      // PUT: api/seasons/5
[HttpPut("{id}")]
public async Task<IActionResult> UpdateSeason(int id, Season updatedSeason)
{
    if (id != updatedSeason.SeasonId)
    {
        return BadRequest("Season ID mismatch");
    }

    _context.Entry(updatedSeason).State = EntityState.Modified;

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!_context.Seasons.Any(s => s.SeasonId == id))
        {
            return NotFound();
        }
        else
        {
            throw;
        }
    }

    return NoContent();
}

// DELETE: api/seasons/5
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteSeason(int id)
{
    var season = await _context.Seasons.FindAsync(id);

    if (season == null)
    {
        return NotFound();
    }

    _context.Seasons.Remove(season);
    await _context.SaveChangesAsync();

    return NoContent();
}
    }
}

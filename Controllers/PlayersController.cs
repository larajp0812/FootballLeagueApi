using FootballLeagueApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly LeagueContext _context;

        public PlayersController(LeagueContext context)
        {
            _context = context;
        }

        // GET: api/players
        [HttpGet]
        public async Task<IActionResult> GetPlayers()
        {
            var players = await _context.Players
                .Include(p => p.Team)
                .ToListAsync();

            return Ok(players);
        }

        // GET: api/players/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlayer(int id)
        {
            var player = await _context.Players
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p => p.PlayerId == id);

            if (player == null)
                return NotFound();

            return Ok(player);
        }

        // POST: api/players
        [HttpPost]
        public async Task<IActionResult> CreatePlayer(Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlayer), new { id = player.PlayerId }, player);
        }

        // PUT: api/players/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlayer(int id, Player updatedPlayer)
        {
            if (id != updatedPlayer.PlayerId)
                return BadRequest("Player ID mismatch");

            _context.Entry(updatedPlayer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Players.Any(p => p.PlayerId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/players/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            var player = await _context.Players.FindAsync(id);

            if (player == null)
                return NotFound();

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

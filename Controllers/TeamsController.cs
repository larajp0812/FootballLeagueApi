using FootballLeagueApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly LeagueContext _context;

        public TeamsController(LeagueContext context)
        {
            _context = context;
        }

        // GET: api/teams
        [HttpGet]
        public async Task<IActionResult> GetTeams()
        {
            var teams = await _context.Teams.ToListAsync();
            return Ok(teams);
        }

        // GET: api/teams/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeam(int id)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
                return NotFound();

            return Ok(team);
        }

        // POST: api/teams
        [HttpPost]
        public async Task<IActionResult> CreateTeam(Team team)
        {
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeam), new { id = team.TeamId }, team);
        }

        // PUT: api/teams/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(int id, Team updatedTeam)
        {
            if (id != updatedTeam.TeamId)
                return BadRequest("Team ID mismatch");

            _context.Entry(updatedTeam).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Teams.Any(t => t.TeamId == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/teams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
                return NotFound();

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

using FootballLeagueApi.Models;
using FootballLeagueApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teams = await _teamService.GetAllAsync();
            return Ok(teams);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var team = await _teamService.GetByIdAsync(id);
            if (team == null)
                return NotFound();

            return Ok(team);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Team team)
        {
            var created = await _teamService.CreateAsync(team);
            return CreatedAtAction(nameof(GetById), new { id = created.TeamId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Team team)
        {
            var success = await _teamService.UpdateAsync(id, team);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _teamService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}

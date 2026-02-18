using FootballLeagueApi.Models;
using FootballLeagueApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchService _matchService;

        public MatchesController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var matches = await _matchService.GetAllAsync();
            return Ok(matches);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var match = await _matchService.GetByIdAsync(id);
            if (match == null)
                return NotFound();

            return Ok(match);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Match match)
        {
            try
            {
                var created = await _matchService.CreateAsync(match);
                return CreatedAtAction(nameof(GetById), new { id = created.MatchId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Match match)
        {
            var success = await _matchService.UpdateAsync(id, match);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _matchService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}

using FootballLeagueApi.Models;
using FootballLeagueApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeasonsController : ControllerBase
    {
        private readonly ISeasonService _seasonService;

        public SeasonsController(ISeasonService seasonService)
        {
            _seasonService = seasonService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var seasons = await _seasonService.GetAllAsync();
            return Ok(seasons);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var season = await _seasonService.GetByIdAsync(id);
            if (season == null)
                return NotFound();

            return Ok(season);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Season season)
        {
            var created = await _seasonService.CreateAsync(season);
            return CreatedAtAction(nameof(GetById), new { id = created.SeasonId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Season season)
        {
            var success = await _seasonService.UpdateAsync(id, season);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _seasonService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}

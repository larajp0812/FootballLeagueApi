using FootballLeagueApi.Models;
using FootballLeagueApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayersController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var players = await _playerService.GetAllAsync();
            return Ok(players);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var player = await _playerService.GetByIdAsync(id);
            if (player == null)
                return NotFound();

            return Ok(player);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Player player)
        {
            var created = await _playerService.CreateAsync(player);
            return CreatedAtAction(nameof(GetById), new { id = created.PlayerId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Player player)
        {
            var success = await _playerService.UpdateAsync(id, player);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _playerService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}

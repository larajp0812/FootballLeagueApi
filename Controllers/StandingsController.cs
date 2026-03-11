using FootballLeagueApi.DTOs;
using FootballLeagueApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StandingsController : ControllerBase
    {
        private readonly IStandingsService _standingsService;

        public StandingsController(IStandingsService standingsService)
        {
            _standingsService = standingsService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StandingsReadDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTable([FromQuery] int? seasonId)
        {
            var table = await _standingsService.GetTableAsync(seasonId);
            return Ok(table);
        }
    }
}

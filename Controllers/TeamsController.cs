using FootballLeagueApi.DTOs;
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
            var dto = teams.Select(t => TeamMapper.ToDto(t));
            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var team = await _teamService.GetByIdAsync(id);
            if (team == null)
                return NotFound();

            return Ok(TeamMapper.ToDto(team));
        }

        [HttpPost]
        public async Task<IActionResult> Create(TeamCreateDto dto)
        {
            var team = TeamMapper.ToModel(dto);
            var created = await _teamService.CreateAsync(team);

            return CreatedAtAction(nameof(GetById), new { id = created.TeamId }, TeamMapper.ToDto(created));
        }
    }
}

using FootballLeagueApi.Models;
using FootballLeagueApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballLeagueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchEventsController : ControllerBase
    {
        private readonly IMatchEventService _eventService;

        public MatchEventsController(IMatchEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var eventsList = await _eventService.GetAllAsync();
            return Ok(eventsList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var matchEvent = await _eventService.GetByIdAsync(id);
            if (matchEvent == null)
                return NotFound();

            return Ok(matchEvent);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MatchEvent matchEvent)
        {
            try
            {
                var created = await _eventService.CreateAsync(matchEvent);
                return CreatedAtAction(nameof(GetById), new { id = created.MatchEventId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MatchEvent matchEvent)
        {
            var success = await _eventService.UpdateAsync(id, matchEvent);
            if (!success)
                return BadRequest();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _eventService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}

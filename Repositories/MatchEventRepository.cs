using FootballLeagueApi.Data;
using FootballLeagueApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueApi.Repositories
{
    public class MatchEventRepository : IMatchEventRepository
    {
        private readonly LeagueContext _context;

        public MatchEventRepository(LeagueContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MatchEvent>> GetAllAsync()
        {
            return await _context.MatchEvents
                .Include(e => e.Match)
                .Include(e => e.Player)
                .ToListAsync();
        }

        public async Task<MatchEvent?> GetByIdAsync(int id)
        {
            return await _context.MatchEvents
                .Include(e => e.Match)
                .Include(e => e.Player)
                .FirstOrDefaultAsync(e => e.MatchEventId == id);
        }

        public async Task AddAsync(MatchEvent matchEvent)
        {
            await _context.MatchEvents.AddAsync(matchEvent);
        }

        public async Task UpdateAsync(MatchEvent matchEvent)
        {
            _context.MatchEvents.Update(matchEvent);
        }

        public async Task DeleteAsync(MatchEvent matchEvent)
        {
            _context.MatchEvents.Remove(matchEvent);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

using FootballLeagueApi.Data;
using FootballLeagueApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueApi.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        private readonly LeagueContext _context;

        public MatchRepository(LeagueContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Match>> GetAllAsync()
        {
            return await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.Season)
                .ToListAsync();
        }

        public async Task<Match?> GetByIdAsync(int id)
        {
            return await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.Season)
                .FirstOrDefaultAsync(m => m.MatchId == id);
        }

        public async Task AddAsync(Match match)
        {
            await _context.Matches.AddAsync(match);
        }

        public async Task UpdateAsync(Match match)
        {
            _context.Matches.Update(match);
        }

        public async Task DeleteAsync(Match match)
        {
            _context.Matches.Remove(match);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

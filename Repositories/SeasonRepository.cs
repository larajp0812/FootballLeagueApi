using FootballLeagueApi.Models;
using FootballLeagueApi.Data;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueApi.Repositories
{
    public class SeasonRepository : ISeasonRepository
    {
        private readonly LeagueContext _context;

        public SeasonRepository(LeagueContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Season>> GetAllAsync()
        {
            return await _context.Seasons.ToListAsync();
        }

        public async Task<Season?> GetByIdAsync(int id)
        {
            return await _context.Seasons.FindAsync(id);
        }

        public async Task AddAsync(Season season)
        {
            await _context.Seasons.AddAsync(season);
        }

        public async Task UpdateAsync(Season season)
        {
            _context.Seasons.Update(season);
        }

        public async Task DeleteAsync(Season season)
        {
            _context.Seasons.Remove(season);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

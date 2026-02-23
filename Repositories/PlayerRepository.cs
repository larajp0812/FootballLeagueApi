using FootballLeagueApi.Data;
using FootballLeagueApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueApi.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly LeagueContext _context;

        public PlayerRepository(LeagueContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Player>> GetAllAsync()
        {
            return await _context.Players.ToListAsync();
        }

        public async Task<Player?> GetByIdAsync(int id)
        {
            return await _context.Players.FindAsync(id);
        }

        public async Task AddAsync(Player player)
        {
            await _context.Players.AddAsync(player);
        }

        public Task UpdateAsync(Player player)
        {
            _context.Players.Update(player);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Player player)
        {
            _context.Players.Remove(player);
            return Task.CompletedTask;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

using FootballLeagueApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueApi.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly LeagueContext _context;

        public TeamRepository(LeagueContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Team>> GetAllAsync()
        {
            return await _context.Teams.ToListAsync();
        }

        public async Task<Team?> GetByIdAsync(int id)
        {
            return await _context.Teams.FindAsync(id);
        }

        public async Task AddAsync(Team team)
        {
            await _context.Teams.AddAsync(team);
        }

        public async Task UpdateAsync(Team team)
        {
            _context.Teams.Update(team);
        }

        public async Task DeleteAsync(Team team)
        {
            _context.Teams.Remove(team);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

using FootballLeagueApi.Models;

namespace FootballLeagueApi.Repositories
{
    public interface IMatchRepository
    {
        Task<IEnumerable<Match>> GetAllAsync();
        Task<Match?> GetByIdAsync(int id);
        Task AddAsync(Match match);
        Task UpdateAsync(Match match);
        Task DeleteAsync(Match match);
        Task<bool> SaveChangesAsync();
    }
}

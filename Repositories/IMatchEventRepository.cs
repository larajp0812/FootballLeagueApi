using FootballLeagueApi.Models;

namespace FootballLeagueApi.Repositories
{
    public interface IMatchEventRepository
    {
        Task<IEnumerable<MatchEvent>> GetAllAsync();
        Task<MatchEvent?> GetByIdAsync(int id);
        Task AddAsync(MatchEvent matchEvent);
        Task UpdateAsync(MatchEvent matchEvent);
        Task DeleteAsync(MatchEvent matchEvent);
        Task<bool> SaveChangesAsync();
    }
}

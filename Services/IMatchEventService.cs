using FootballLeagueApi.Models;

namespace FootballLeagueApi.Services
{
    public interface IMatchEventService
    {
        Task<IEnumerable<MatchEvent>> GetAllAsync();
        Task<MatchEvent?> GetByIdAsync(int id);
        Task<MatchEvent> CreateAsync(MatchEvent matchEvent);
        Task<bool> UpdateAsync(int id, MatchEvent matchEvent);
        Task<bool> DeleteAsync(int id);
    }
}

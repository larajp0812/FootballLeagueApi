using FootballLeagueApi.Models;

namespace FootballLeagueApi.Services
{
    public interface IMatchService
    {
        Task<IEnumerable<Match>> GetAllAsync();
        Task<Match?> GetByIdAsync(int id);
        Task<Match> CreateAsync(Match match);
        Task<bool> UpdateAsync(int id, Match match);
        Task<bool> DeleteAsync(int id);
    }
}

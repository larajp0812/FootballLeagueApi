using FootballLeagueApi.Models;

namespace FootballLeagueApi.Services
{
    public interface ISeasonService
    {
        Task<IEnumerable<Season>> GetAllAsync();
        Task<Season?> GetByIdAsync(int id);
        Task<Season> CreateAsync(Season season);
        Task<bool> UpdateAsync(int id, Season season);
        Task<bool> DeleteAsync(int id);
    }
}

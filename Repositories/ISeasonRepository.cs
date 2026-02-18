using FootballLeagueApi.Models;

namespace FootballLeagueApi.Repositories
{
    public interface ISeasonRepository
    {
        Task<IEnumerable<Season>> GetAllAsync();
        Task<Season?> GetByIdAsync(int id);
        Task AddAsync(Season season);
        Task UpdateAsync(Season season);
        Task DeleteAsync(Season season);
        Task<bool> SaveChangesAsync();
    }
}

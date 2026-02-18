using FootballLeagueApi.Models;

namespace FootballLeagueApi.Services
{
    public interface IPlayerService
    {
        Task<IEnumerable<Player>> GetAllAsync();
        Task<Player?> GetByIdAsync(int id);
        Task<Player> CreateAsync(Player player);
        Task<bool> UpdateAsync(int id, Player player);
        Task<bool> DeleteAsync(int id);
    }
}

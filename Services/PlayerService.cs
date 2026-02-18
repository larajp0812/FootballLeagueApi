using FootballLeagueApi.Models;
using FootballLeagueApi.Repositories;

namespace FootballLeagueApi.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _repo;

        public PlayerService(IPlayerRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Player>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Player?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<Player> CreateAsync(Player player)
        {
            await _repo.AddAsync(player);
            await _repo.SaveChangesAsync();
            return player;
        }

        public async Task<bool> UpdateAsync(int id, Player player)
        {
            if (id != player.PlayerId)
                return false;

            await _repo.UpdateAsync(player);
            return await _repo.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var player = await _repo.GetByIdAsync(id);
            if (player == null)
                return false;

            await _repo.DeleteAsync(player);
            return await _repo.SaveChangesAsync();
        }
    }
}

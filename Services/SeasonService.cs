using FootballLeagueApi.Models;
using FootballLeagueApi.Repositories;

namespace FootballLeagueApi.Services
{
    public class SeasonService : ISeasonService
    {
        private readonly ISeasonRepository _repo;

        public SeasonService(ISeasonRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Season>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Season?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<Season> CreateAsync(Season season)
        {
            await _repo.AddAsync(season);
            await _repo.SaveChangesAsync();
            return season;
        }

        public async Task<bool> UpdateAsync(int id, Season season)
        {
            if (id != season.SeasonId)
                return false;

            await _repo.UpdateAsync(season);
            return await _repo.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var season = await _repo.GetByIdAsync(id);
            if (season == null)
                return false;

            await _repo.DeleteAsync(season);
            return await _repo.SaveChangesAsync();
        }
    }
}

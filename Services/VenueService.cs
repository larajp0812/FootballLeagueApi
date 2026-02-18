using FootballLeagueApi.Models;
using FootballLeagueApi.Repositories;

namespace FootballLeagueApi.Services
{
    public class VenueService : IVenueService
    {
        private readonly IVenueRepository _repo;

        public VenueService(IVenueRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Venue>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Venue?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<Venue> CreateAsync(Venue venue)
        {
            await _repo.AddAsync(venue);
            await _repo.SaveChangesAsync();
            return venue;
        }

        public async Task<bool> UpdateAsync(int id, Venue venue)
        {
            if (id != venue.VenueId)
                return false;

            await _repo.UpdateAsync(venue);
            return await _repo.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var venue = await _repo.GetByIdAsync(id);
            if (venue == null)
                return false;

            await _repo.DeleteAsync(venue);
            return await _repo.SaveChangesAsync();
        }
    }
}

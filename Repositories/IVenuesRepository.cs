using FootballLeagueApi.Models;

namespace FootballLeagueApi.Repositories
{
    public interface IVenueRepository
    {
        Task<IEnumerable<Venue>> GetAllAsync();
        Task<Venue?> GetByIdAsync(int id);
        Task AddAsync(Venue venue);
        Task UpdateAsync(Venue venue);
        Task DeleteAsync(Venue venue);
        Task<bool> SaveChangesAsync();
    }
}

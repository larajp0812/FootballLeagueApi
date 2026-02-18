using FootballLeagueApi.Models;

namespace FootballLeagueApi.Services
{
    public interface IVenueService
    {
        Task<IEnumerable<Venue>> GetAllAsync();
        Task<Venue?> GetByIdAsync(int id);
        Task<Venue> CreateAsync(Venue venue);
        Task<bool> UpdateAsync(int id, Venue venue);
        Task<bool> DeleteAsync(int id);
    }
}

using FootballLeagueApi.Data;
using FootballLeagueApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballLeagueApi.Repositories
{
    public class VenueRepository : IVenueRepository
    {
        private readonly LeagueContext _context;

        public VenueRepository(LeagueContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Venue>> GetAllAsync()
        {
            return await _context.Venues.ToListAsync();
        }

        public async Task<Venue?> GetByIdAsync(int id)
        {
            return await _context.Venues.FindAsync(id);
        }

        public async Task AddAsync(Venue venue)
        {
            await _context.Venues.AddAsync(venue);
        }

        public async Task UpdateAsync(Venue venue)
        {
            _context.Venues.Update(venue);
        }

        public async Task DeleteAsync(Venue venue)
        {
            _context.Venues.Remove(venue);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

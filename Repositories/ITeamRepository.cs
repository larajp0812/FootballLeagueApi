using FootballLeagueApi.Models;

namespace FootballLeagueApi.Repositories
{
    public interface ITeamRepository
    {
        Task<IEnumerable<Team>> GetAllAsync();
        Task<Team?> GetByIdAsync(int id);
        Task AddAsync(Team team);
        Task UpdateAsync(Team team);
        Task DeleteAsync(Team team);
        Task<bool> SaveChangesAsync();
    }
}

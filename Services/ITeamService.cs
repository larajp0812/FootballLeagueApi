using FootballLeagueApi.Models;

namespace FootballLeagueApi.Services
{
    public interface ITeamService
    {
        Task<IEnumerable<Team>> GetAllAsync();
        Task<Team?> GetByIdAsync(int id);
        Task<Team> CreateAsync(Team team);
        Task<bool> UpdateAsync(int id, Team team);
        Task<bool> DeleteAsync(int id);
    }
}

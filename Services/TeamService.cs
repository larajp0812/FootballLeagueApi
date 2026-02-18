using FootballLeagueApi.Models;
using FootballLeagueApi.Repositories;

namespace FootballLeagueApi.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _repo;

        public TeamService(ITeamRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Team>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Team?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<Team> CreateAsync(Team team)
        {
            await _repo.AddAsync(team);
            await _repo.SaveChangesAsync();
            return team;
        }

        public async Task<bool> UpdateAsync(int id, Team team)
        {
            if (id != team.TeamId)
                return false;

            await _repo.UpdateAsync(team);
            return await _repo.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var team = await _repo.GetByIdAsync(id);
            if (team == null)
                return false;

            await _repo.DeleteAsync(team);
            return await _repo.SaveChangesAsync();
        }
    }
}

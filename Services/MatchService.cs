using FootballLeagueApi.Models;
using FootballLeagueApi.Repositories;

namespace FootballLeagueApi.Services
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepo;
        private readonly ITeamRepository _teamRepo;
        private readonly ISeasonRepository _seasonRepo;
        private readonly IVenueRepository _venueRepo;

        public MatchService(
            IMatchRepository matchRepo,
            ITeamRepository teamRepo,
            ISeasonRepository seasonRepo,
            IVenueRepository venueRepo)
        {
            _matchRepo = matchRepo;
            _teamRepo = teamRepo;
            _seasonRepo = seasonRepo;
            _venueRepo = venueRepo;
        }

        public async Task<IEnumerable<Match>> GetAllAsync()
        {
            return await _matchRepo.GetAllAsync();
        }

        public async Task<Match?> GetByIdAsync(int id)
        {
            return await _matchRepo.GetByIdAsync(id);
        }

        public async Task<Match> CreateAsync(Match match)
        {
            // Business rules
            if (match.HomeTeamId == match.AwayTeamId)
                throw new Exception("Home and Away teams cannot be the same.");

            if (await _teamRepo.GetByIdAsync(match.HomeTeamId) == null)
                throw new Exception("Home team does not exist.");

            if (await _teamRepo.GetByIdAsync(match.AwayTeamId) == null)
                throw new Exception("Away team does not exist.");

            if (await _seasonRepo.GetByIdAsync(match.SeasonId) == null)
                throw new Exception("Season does not exist.");

            if (await _venueRepo.GetByIdAsync(match.VenueId) == null)
                throw new Exception("Venue does not exist.");

            await _matchRepo.AddAsync(match);
            await _matchRepo.SaveChangesAsync();
            return match;
        }

        public async Task<bool> UpdateAsync(int id, Match match)
        {
            if (id != match.MatchId)
                return false;

            await _matchRepo.UpdateAsync(match);
            return await _matchRepo.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var match = await _matchRepo.GetByIdAsync(id);
            if (match == null)
                return false;

            await _matchRepo.DeleteAsync(match);
            return await _matchRepo.SaveChangesAsync();
        }
    }
}

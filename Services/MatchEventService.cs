using FootballLeagueApi.Models;
using FootballLeagueApi.Repositories;

namespace FootballLeagueApi.Services
{
    public class MatchEventService : IMatchEventService
    {
        private readonly IMatchEventRepository _eventRepo;
        private readonly IMatchRepository _matchRepo;
        private readonly IPlayerRepository _playerRepo;

        public MatchEventService(
            IMatchEventRepository eventRepo,
            IMatchRepository matchRepo,
            IPlayerRepository playerRepo)
        {
            _eventRepo = eventRepo;
            _matchRepo = matchRepo;
            _playerRepo = playerRepo;
        }

        public async Task<IEnumerable<MatchEvent>> GetAllAsync()
        {
            return await _eventRepo.GetAllAsync();
        }

        public async Task<MatchEvent?> GetByIdAsync(int id)
        {
            return await _eventRepo.GetByIdAsync(id);
        }

public async Task<MatchEvent> CreateAsync(MatchEvent matchEvent)
{
    // Validation
    if (await _matchRepo.GetByIdAsync(matchEvent.MatchId) == null)
        throw new Exception("Match does not exist.");

    if (matchEvent.PlayerId != null)
    {
        if (await _playerRepo.GetByIdAsync(matchEvent.PlayerId.Value) == null)
            throw new Exception("Player does not exist.");
    }

    await _eventRepo.AddAsync(matchEvent);
    await _eventRepo.SaveChangesAsync();
    return matchEvent;
}


        public async Task<bool> UpdateAsync(int id, MatchEvent matchEvent)
        {
            if (id != matchEvent.MatchEventId)
                return false;

            await _eventRepo.UpdateAsync(matchEvent);
            return await _eventRepo.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var matchEvent = await _eventRepo.GetByIdAsync(id);
            if (matchEvent == null)
                return false;

            await _eventRepo.DeleteAsync(matchEvent);
            return await _eventRepo.SaveChangesAsync();
        }
    }
}

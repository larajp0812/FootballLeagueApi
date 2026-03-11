using FootballLeagueApi.DTOs;

namespace FootballLeagueApi.Services
{
    public interface IStandingsService
    {
        Task<IEnumerable<StandingsReadDto>> GetTableAsync(int? seasonId = null);
    }
}

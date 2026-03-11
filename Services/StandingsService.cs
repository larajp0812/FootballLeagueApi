using FootballLeagueApi.DTOs;
using FootballLeagueApi.Repositories;

namespace FootballLeagueApi.Services
{
    public class StandingsService : IStandingsService
    {
        private readonly IMatchRepository _matchRepository;
        private readonly ITeamRepository _teamRepository;

        public StandingsService(IMatchRepository matchRepository, ITeamRepository teamRepository)
        {
            _matchRepository = matchRepository;
            _teamRepository = teamRepository;
        }

        public async Task<IEnumerable<StandingsReadDto>> GetTableAsync(int? seasonId = null)
        {
            var teams = (await _teamRepository.GetAllAsync()).ToList();
            var matches = (await _matchRepository.GetAllAsync()).ToList();

            if (seasonId.HasValue)
            {
                matches = matches.Where(match => match.SeasonId == seasonId.Value).ToList();
            }

            var now = DateTime.UtcNow;
            var playedMatches = matches.Where(match =>
                match.KickoffTime <= now ||
                match.HomeScore != 0 ||
                match.AwayScore != 0).ToList();

            var table = teams.ToDictionary(
                team => team.TeamId,
                team => new StandingsReadDto
                {
                    TeamId = team.TeamId,
                    TeamName = team.Name,
                });

            foreach (var match in playedMatches)
            {
                if (!table.TryGetValue(match.HomeTeamId, out var homeRow) ||
                    !table.TryGetValue(match.AwayTeamId, out var awayRow))
                {
                    continue;
                }

                var homeScore = match.HomeScore;
                var awayScore = match.AwayScore;

                homeRow.Played++;
                awayRow.Played++;

                homeRow.GoalsFor += homeScore;
                homeRow.GoalsAgainst += awayScore;

                awayRow.GoalsFor += awayScore;
                awayRow.GoalsAgainst += homeScore;

                if (homeScore > awayScore)
                {
                    homeRow.Won++;
                    homeRow.Points += 3;
                    awayRow.Lost++;
                }
                else if (homeScore < awayScore)
                {
                    awayRow.Won++;
                    awayRow.Points += 3;
                    homeRow.Lost++;
                }
                else
                {
                    homeRow.Drawn++;
                    awayRow.Drawn++;
                    homeRow.Points++;
                    awayRow.Points++;
                }
            }

            foreach (var row in table.Values)
            {
                row.GoalDifference = row.GoalsFor - row.GoalsAgainst;
            }

            var ordered = table.Values
                .OrderByDescending(row => row.Points)
                .ThenByDescending(row => row.GoalDifference)
                .ThenByDescending(row => row.GoalsFor)
                .ThenBy(row => row.TeamName)
                .ToList();

            for (var index = 0; index < ordered.Count; index++)
            {
                ordered[index].Position = index + 1;
            }

            return ordered;
        }
    }
}

using FootballLeagueApi.Models;

namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// MatchMapper - Utility class for converting between Match DTOs and domain models
    /// </summary>
    public static class MatchMapper
    {
        /// <summary>
        /// Convert a Match domain model to a MatchReadDto
        /// Used when returning match data in API responses
        /// </summary>
        public static MatchReadDto ToDto(Match match)
        {
            return new MatchReadDto
            {
                MatchId = match.MatchId,
                HomeTeamId = match.HomeTeamId,
                AwayTeamId = match.AwayTeamId,
                SeasonId = match.SeasonId,
                Venue = match.HomeTeam?.Venue ?? string.Empty,
                KickoffTime = match.KickoffTime,
                HomeScore = match.HomeScore,
                AwayScore = match.AwayScore
            };
        }

        /// <summary>
        /// Convert a MatchCreateDto to a Match domain model
        /// Used when creating a new match from API request data
        /// </summary>
        public static Match ToModel(MatchCreateDto dto)
        {
            return new Match
            {
                HomeTeamId = dto.HomeTeamId,
                AwayTeamId = dto.AwayTeamId,
                SeasonId = dto.SeasonId,
                KickoffTime = dto.KickoffTime,
                HomeScore = dto.HomeScore,
                AwayScore = dto.AwayScore
            };
        }

        /// <summary>
        /// Convert a MatchUpdateDto to a Match domain model
        /// Used when updating an existing match
        /// </summary>
        public static Match ToModel(MatchUpdateDto dto, int matchId)
        {
            return new Match
            {
                MatchId = matchId,
                HomeTeamId = dto.HomeTeamId,
                AwayTeamId = dto.AwayTeamId,
                SeasonId = dto.SeasonId,
                KickoffTime = dto.KickoffTime,
                HomeScore = dto.HomeScore,
                AwayScore = dto.AwayScore
            };
        }
    }
}

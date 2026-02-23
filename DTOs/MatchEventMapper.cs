using FootballLeagueApi.Models;

namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// MatchEventMapper - Utility class for converting between MatchEvent DTOs and domain models
    /// </summary>
    public static class MatchEventMapper
    {
        /// <summary>
        /// Convert a MatchEvent domain model to a MatchEventReadDto
        /// Used when returning match event data in API responses
        /// </summary>
        public static MatchEventReadDto ToDto(MatchEvent matchEvent)
        {
            return new MatchEventReadDto
            {
                MatchEventId = matchEvent.MatchEventId,
                MatchId = matchEvent.MatchId,
                PlayerId = matchEvent.PlayerId,
                Minute = matchEvent.Minute,
                EventType = matchEvent.EventType
            };
        }

        /// <summary>
        /// Convert a MatchEventCreateDto to a MatchEvent domain model
        /// Used when creating a new match event from API request data
        /// </summary>
        public static MatchEvent ToModel(MatchEventCreateDto dto)
        {
            return new MatchEvent
            {
                MatchId = dto.MatchId,
                PlayerId = dto.PlayerId,
                Minute = dto.Minute,
                EventType = dto.EventType
            };
        }

        /// <summary>
        /// Convert a MatchEventUpdateDto to a MatchEvent domain model
        /// Used when updating an existing match event
        /// </summary>
        public static MatchEvent ToModel(MatchEventUpdateDto dto, int matchEventId, int matchId)
        {
            return new MatchEvent
            {
                MatchEventId = matchEventId,
                MatchId = matchId,
                PlayerId = dto.PlayerId,
                Minute = dto.Minute,
                EventType = dto.EventType
            };
        }
    }
}

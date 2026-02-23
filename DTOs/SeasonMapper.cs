using FootballLeagueApi.Models;

namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// SeasonMapper - Utility class for converting between Season DTOs and domain models
    /// </summary>
    public static class SeasonMapper
    {
        /// <summary>
        /// Convert a Season domain model to a SeasonReadDto
        /// Used when returning season data in API responses
        /// </summary>
        public static SeasonReadDto ToDto(Season season)
        {
            return new SeasonReadDto
            {
                SeasonId = season.SeasonId,
                Name = season.Name,
                StartDate = season.StartDate,
                EndDate = season.EndDate
            };
        }

        /// <summary>
        /// Convert a SeasonCreateDto to a Season domain model
        /// Used when creating a new season from API request data
        /// </summary>
        public static Season ToModel(SeasonCreateDto dto)
        {
            return new Season
            {
                Name = dto.Name,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };
        }

        /// <summary>
        /// Convert a SeasonUpdateDto to a Season domain model
        /// Used when updating an existing season
        /// </summary>
        public static Season ToModel(SeasonUpdateDto dto, int seasonId)
        {
            return new Season
            {
                SeasonId = seasonId,
                Name = dto.Name,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };
        }
    }
}

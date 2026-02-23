using FootballLeagueApi.Models;

namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// VenueMapper - Utility class for converting between Venue DTOs and domain models
    /// </summary>
    public static class VenueMapper
    {
        /// <summary>
        /// Convert a Venue domain model to a VenueReadDto
        /// Used when returning venue data in API responses
        /// </summary>
        public static VenueReadDto ToDto(Venue venue)
        {
            return new VenueReadDto
            {
                VenueId = venue.VenueId,
                Name = venue.Name,
                Address = venue.Address
            };
        }

        /// <summary>
        /// Convert a VenueCreateDto to a Venue domain model
        /// Used when creating a new venue from API request data
        /// </summary>
        public static Venue ToModel(VenueCreateDto dto)
        {
            return new Venue
            {
                Name = dto.Name,
                Address = dto.Address
            };
        }

        /// <summary>
        /// Convert a VenueUpdateDto to a Venue domain model
        /// Used when updating an existing venue
        /// </summary>
        public static Venue ToModel(VenueUpdateDto dto, int venueId)
        {
            return new Venue
            {
                VenueId = venueId,
                Name = dto.Name,
                Address = dto.Address
            };
        }
    }
}

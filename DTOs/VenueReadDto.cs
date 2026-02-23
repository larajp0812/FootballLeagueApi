namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// VenueReadDto - Data Transfer Object for reading venue data
    /// 
    /// Used in API responses (GET requests) to return venue information.
    /// Includes the VenueId and all venue details.
    /// </summary>
    public class VenueReadDto
    {
        /// <summary>
        /// The unique identifier for the venue
        /// </summary>
        public int VenueId { get; set; }

        /// <summary>
        /// The name of the venue/stadium
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The physical address of the venue
        /// </summary>
        public string? Address { get; set; }
    }
}

namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// VenueUpdateDto - Data Transfer Object for updating a venue
    /// 
    /// Used in PUT /api/venues/{id} requests to update venue information.
    /// The VenueId is not included as it comes from the URL path.
    /// </summary>
    public class VenueUpdateDto
    {
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

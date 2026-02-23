namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// VenueCreateDto - Data Transfer Object for creating a new venue
    /// 
    /// Used in POST /api/venues requests to add a new stadium/venue to the system.
    /// </summary>
    public class VenueCreateDto
    {
        /// <summary>
        /// The name of the venue/stadium (e.g., "Old Trafford", "Anfield")
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The physical address of the venue
        /// </summary>
        public string? Address { get; set; }
    }
}

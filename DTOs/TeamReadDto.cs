namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// TeamReadDto - Data Transfer Object for reading team data
    /// 
    /// This DTO represents the data structure returned in GET responses.
    /// It includes the TeamId because the client needs to identify the team.
    /// This is used when returning teams to the client in API responses.
    /// </summary>
    public class TeamReadDto
    {
        /// <summary>
        /// The unique identifier for the team
        /// Populated by the database and included in read responses
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        /// The name of the team
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The name of the team's head coach
        /// </summary>
        public string Coach { get; set; }

        /// <summary>
        /// The year the team was founded
        /// </summary>
        public int FoundedYear { get; set; }

        /// <summary>
        /// The home venue/stadium for the team
        /// </summary>
        public string Venue { get; set; }
    }
}

namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// SeasonReadDto - Data Transfer Object for reading season data
    /// 
    /// Used in API responses (GET requests) to return season information.
    /// Includes the SeasonId and all season details.
    /// </summary>
    public class SeasonReadDto
    {
        /// <summary>
        /// The unique identifier for the season
        /// </summary>
        public int SeasonId { get; set; }

        /// <summary>
        /// The name/identifier for the season
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The date when this season starts
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The date when this season ends
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}

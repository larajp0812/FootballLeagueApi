namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// SeasonCreateDto - Data Transfer Object for creating a new season
    /// 
    /// Used in POST /api/seasons requests to create a new league season.
    /// </summary>
    public class SeasonCreateDto
    {
        /// <summary>
        /// The name/identifier for the season (e.g., "2025/26", "Premier League 2025-26")
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

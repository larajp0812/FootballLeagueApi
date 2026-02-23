namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// SeasonUpdateDto - Data Transfer Object for updating a season
    /// 
    /// Used in PUT /api/seasons/{id} requests to update season information.
    /// The SeasonId is not included as it comes from the URL path.
    /// </summary>
    public class SeasonUpdateDto
    {
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

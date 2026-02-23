namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// MatchEventUpdateDto - Data Transfer Object for updating a match event
    /// 
    /// Used in PUT /api/matchevents/{id} requests to update event details.
    /// The MatchEventId is not included as it comes from the URL path.
    /// </summary>
    public class MatchEventUpdateDto
    {
        /// <summary>
        /// The minute when the event occurred
        /// </summary>
        public int Minute { get; set; }

        /// <summary>
        /// The type of event
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// The ID of the player involved (optional)
        /// </summary>
        public int? PlayerId { get; set; }
    }
}

namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// MatchEventReadDto - Data Transfer Object for reading match event data
    /// 
    /// Used in API responses (GET requests) to return match event information.
    /// Includes all event details including the match and player references.
    /// </summary>
    public class MatchEventReadDto
    {
        /// <summary>
        /// The unique identifier for the match event
        /// </summary>
        public int MatchEventId { get; set; }

        /// <summary>
        /// The ID of the match this event occurred in
        /// </summary>
        public int MatchId { get; set; }

        /// <summary>
        /// The ID of the player involved (if applicable)
        /// </summary>
        public int? PlayerId { get; set; }

        /// <summary>
        /// The minute when the event occurred
        /// </summary>
        public int Minute { get; set; }

        /// <summary>
        /// The type of event
        /// </summary>
        public string EventType { get; set; }
    }
}

namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// MatchEventCreateDto - Data Transfer Object for creating a new match event
    /// 
    /// Used in POST /api/matchevents requests to record an event during a match.
    /// Events can be goals, cards, substitutions, etc.
    /// </summary>
    public class MatchEventCreateDto
    {
        /// <summary>
        /// The ID of the match this event occurred in
        /// </summary>
        public int MatchId { get; set; }

        /// <summary>
        /// The ID of the player involved in this event (optional)
        /// Nullable for events that don't involve a specific player
        /// </summary>
        public int? PlayerId { get; set; }

        /// <summary>
        /// The minute (game time) when the event occurred
        /// </summary>
        public int Minute { get; set; }

        /// <summary>
        /// The type of event (e.g., "Goal", "YellowCard", "RedCard", "Substitution", "OwnGoal")
        /// </summary>
        public string EventType { get; set; }
    }
}

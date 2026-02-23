namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// MatchUpdateDto - Data Transfer Object for updating match data
    /// 
    /// Used in PUT /api/matches/{id} requests to update match information.
    /// Typically used to update final scores after a match completes.
    /// The MatchId is not included as it comes from the URL path.
    /// </summary>
    public class MatchUpdateDto
    {
        /// <summary>
        /// The final score for the home team
        /// </summary>
        public int HomeScore { get; set; }

        /// <summary>
        /// The final score for the away team
        /// </summary>
        public int AwayScore { get; set; }

        /// <summary>
        /// The kickoff time (can be rescheduled)
        /// </summary>
        public DateTime KickoffTime { get; set; }
    }
}

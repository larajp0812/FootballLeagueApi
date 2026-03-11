namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// MatchUpdateDto - Data Transfer Object for updating match data
    /// 
    /// Used in PUT /api/matches/{id} requests to update match information.
    /// Supports updating teams, season, kickoff time, and scores.
    /// The MatchId is not included as it comes from the URL path.
    /// </summary>
    public class MatchUpdateDto
    {
        /// <summary>
        /// The ID of the team playing at home
        /// </summary>
        public int HomeTeamId { get; set; }

        /// <summary>
        /// The ID of the team playing away
        /// </summary>
        public int AwayTeamId { get; set; }

        /// <summary>
        /// The ID of the season this match belongs to
        /// </summary>
        public int SeasonId { get; set; }

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

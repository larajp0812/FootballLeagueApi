namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// MatchReadDto - Data Transfer Object for reading match data
    /// 
    /// Used in API responses (GET requests) to return match information.
    /// Includes all match details including scores and team references.
    /// </summary>
    public class MatchReadDto
    {
        /// <summary>
        /// The unique identifier for the match
        /// </summary>
        public int MatchId { get; set; }

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
        /// The ID of the venue where this match is played
        /// </summary>
        public int VenueId { get; set; }

        /// <summary>
        /// The date and time when the match kicks off
        /// </summary>
        public DateTime KickoffTime { get; set; }

        /// <summary>
        /// The final score for the home team
        /// </summary>
        public int HomeScore { get; set; }

        /// <summary>
        /// The final score for the away team
        /// </summary>
        public int AwayScore { get; set; }
    }
}

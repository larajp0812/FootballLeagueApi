namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// MatchCreateDto - Data Transfer Object for creating a new match
    /// 
    /// Used in POST /api/matches requests to schedule a new match.
    /// Requires home team, away team, season, venue, and kickoff time.
    /// </summary>
    public class MatchCreateDto
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
        /// The ID of the venue where this match will be played
        /// </summary>
        public int VenueId { get; set; }

        /// <summary>
        /// The date and time when the match kicks off
        /// </summary>
        public DateTime KickoffTime { get; set; }

        /// <summary>
        /// Home team's score for the match
        /// </summary>
        public int HomeScore { get; set; }

        /// <summary>
        /// Away team's score for the match
        /// </summary>
        public int AwayScore { get; set; }
    }
}

namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// TeamUpdateDto - Data Transfer Object for updating a team
    /// 
    /// This DTO represents the data expected in a PUT /api/teams/{id} request body.
    /// The TeamId is NOT included because it comes from the URL path (/api/teams/{id}).
    /// This prevents confusion and accidental ID mismatches.
    /// </summary>
    public class TeamUpdateDto
    {
        /// <summary>
        /// The updated name of the team
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The updated name of the team's head coach
        /// </summary>
        public string Coach { get; set; }

        /// <summary>
        /// The updated year the team was founded
        /// </summary>
        public int FoundedYear { get; set; }
    }
}

namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// PlayerCreateDto - Data Transfer Object for creating a new player
    /// 
    /// Used in POST /api/players requests to create a new player.
    /// The PlayerId is auto-generated; the TeamId is provided to assign the player to a team.
    /// </summary>
    public class PlayerCreateDto
    {
        /// <summary>
        /// The full name of the player
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// The shirt number worn by the player
        /// </summary>
        public int ShirtNumber { get; set; }

        /// <summary>
        /// The player's position on the field (e.g., "Goalkeeper", "Defender", "Midfielder", "Forward")
        /// </summary>
        public string? Position { get; set; }

        /// <summary>
        /// The ID of the team this player will play for
        /// Must reference an existing team in the database
        /// </summary>
        public int TeamId { get; set; }
    }
}

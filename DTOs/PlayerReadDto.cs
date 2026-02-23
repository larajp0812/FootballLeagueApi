namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// PlayerReadDto - Data Transfer Object for reading player data
    /// 
    /// Used in API responses (GET requests) to return player information.
    /// Includes the PlayerId for client reference.
    /// </summary>
    public class PlayerReadDto
    {
        /// <summary>
        /// The unique identifier for the player
        /// </summary>
        public int PlayerId { get; set; }

        /// <summary>
        /// The full name of the player
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// The shirt number worn by the player
        /// </summary>
        public int ShirtNumber { get; set; }

        /// <summary>
        /// The player's position on the field
        /// </summary>
        public string? Position { get; set; }

        /// <summary>
        /// The ID of the team this player plays for
        /// </summary>
        public int TeamId { get; set; }
    }
}

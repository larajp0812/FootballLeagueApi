using System.ComponentModel.DataAnnotations;

namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// PlayerUpdateDto - Data Transfer Object for updating a player
    /// 
    /// Used in PUT /api/players/{id} requests to update player information.
    /// The PlayerId is not included as it comes from the URL path.
    /// </summary>
    public class PlayerUpdateDto
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
        /// The player's position on the field
        /// </summary>
        [Required]
        [AllowedPlayerPosition]
        public string Position { get; set; }
    }
}

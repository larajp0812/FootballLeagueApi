using System.ComponentModel.DataAnnotations;

namespace FootballLeagueApi.DTOs.Auth
{
    /// <summary>
    /// AssignRoleDto - Request payload for assigning a role to a user
    /// </summary>
    public class AssignRoleDto
    {
        /// <summary>
        /// ASP.NET Identity user ID
        /// </summary>
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }

        /// <summary>
        /// Role name to assign
        /// </summary>
        [Required(ErrorMessage = "RoleName is required")]
        public string RoleName { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace FootballLeagueApi.DTOs.Auth
{
    /// <summary>
    /// CreateRoleDto - Request payload for creating a new role
    /// </summary>
    public class CreateRoleDto
    {
        /// <summary>
        /// Name of the role to create (e.g., Admin, User)
        /// </summary>
        [Required(ErrorMessage = "Role name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 50 characters")]
        public string RoleName { get; set; }
    }
}

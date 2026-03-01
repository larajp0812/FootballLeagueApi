using System.ComponentModel.DataAnnotations;

namespace FootballLeagueApi.DTOs.Auth
{
    /// <summary>
    /// UpdateRoleDto - Request payload for renaming an existing role
    /// </summary>
    public class UpdateRoleDto
    {
        /// <summary>
        /// Role ID to update
        /// </summary>
        [Required(ErrorMessage = "RoleId is required")]
        public string RoleId { get; set; }

        /// <summary>
        /// New role name
        /// </summary>
        [Required(ErrorMessage = "NewRoleName is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 50 characters")]
        public string NewRoleName { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace FootballLeagueApi.DTOs.Auth
{
    /// <summary>
    /// LoginDto - Data Transfer Object for user login requests
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// The user's email
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "A valid email is required")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The user's password
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace FootballLeagueApi.DTOs.Auth
{
    /// <summary>
    /// LoginDto - Data Transfer Object for user login requests
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// The user's email address (used as unique identifier)
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+$", ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        /// <summary>
        /// The user's password
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}

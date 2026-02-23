using System.ComponentModel.DataAnnotations;

namespace FootballLeagueApi.DTOs.Auth
{
    /// <summary>
    /// RegisterDto - Data Transfer Object for user registration requests
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// The desired username for the account
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string UserName { get; set; }

        /// <summary>
        /// The user's email address
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        /// <summary>
        /// The user's password
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "Password must contain uppercase, lowercase, number, and special character")]
        public string Password { get; set; }
    }
}

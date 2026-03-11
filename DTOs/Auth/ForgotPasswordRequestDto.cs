using System.ComponentModel.DataAnnotations;

namespace FootballLeagueApi.DTOs.Auth
{
    public class ForgotPasswordRequestDto
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+$", ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;
    }
}

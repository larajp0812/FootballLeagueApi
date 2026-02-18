using System.ComponentModel.DataAnnotations;

namespace FootballLeagueApi.Models
{
    public class RegisterDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}

namespace FootballLeagueApi.DTOs.Auth
{
    public class RegisterDto
    {
        public string UserName { get; set; }   // ← REQUIRED
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

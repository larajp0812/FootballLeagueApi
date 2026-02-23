namespace FootballLeagueApi.DTOs.Auth
{
    /// <summary>
    /// LoginDto - Data Transfer Object for user login requests
    /// 
    /// This DTO represents the data expected in a POST /api/auth/login request.
    /// Users provide their credentials (email and password) to authenticate.
    /// Upon successful authentication, the API returns authentication credentials
    /// (like a JWT token) that can be used for subsequent API requests.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// The user's email address (used as unique identifier)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The user's password (must match the hashed password in the database)
        /// Should never be stored in plaintext - client sends it encrypted over HTTPS
        /// </summary>
        public string Password { get; set; }
    }
}

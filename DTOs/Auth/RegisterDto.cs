namespace FootballLeagueApi.DTOs.Auth
{
    /// <summary>
    /// RegisterDto - Data Transfer Object for user registration requests
    /// 
    /// This DTO represents the data expected in a POST /api/auth/register request.
    /// New users provide this information to create an account in the system.
    /// The UserManager (from ASP.NET Identity) validates and securely stores this data,
    /// automatically hashing the password.
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// The desired username for the account
        /// Must be unique in the system (enforced by Identity)
        /// REQUIRED - cannot be null or empty
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The user's email address
        /// Also enforced as unique by Identity
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The user's password
        /// ASP.NET Identity automatically hashes this password
        /// Never stored in plaintext in the database
        /// The user should send this encrypted via HTTPS
        /// </summary>
        public string Password { get; set; }
    }
}

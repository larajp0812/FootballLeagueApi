using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FootballLeagueApi.DTOs.Auth;

namespace FootballLeagueApi.Controllers
{
    /// <summary>
    /// Auth Controller - Handles user authentication operations
    /// This controller provides REST API endpoints for user registration and login.
    /// Uses ASP.NET Core Identity for secure user management and password hashing.
    /// Maps to /api/auth endpoint.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Dependency-injected UserManager from ASP.NET Core Identity
        /// Provides methods for creating users, checking passwords, and managing user accounts
        /// </summary>
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Constructor accepting UserManager and ILogger through dependency injection
        /// UserManager is provided by the Identity framework and logger for diagnostics
        /// </summary>
        public AuthController(UserManager<IdentityUser> userManager, ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// POST /api/auth/register - Register a new user account
        /// Accepts a RegisterDto with UserName, Email, and Password
        /// Validates input and creates a new user with hashed password
        /// Returns:
        ///   200 OK with success message if registration succeeds
        ///   400 Bad Request with validation errors if registration fails
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            try
            {
                _logger.LogInformation("User registration attempt for email: {Email}", model.Email);
                
                // Check if the model is valid (all required fields present)
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Registration failed: Invalid model state for email {Email}", model.Email);
                    return BadRequest(ModelState);
                }

                // Create a new Identity user with the provided username and email
                var user = new IdentityUser
                {
                    UserName = model.UserName,
                    Email = model.Email
                };

                // Call UserManager to create the user with the hashed password
                // UserManager automatically hashes passwords for security
                var result = await _userManager.CreateAsync(user, model.Password);

                // Check if user creation was successful
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Registration failed for email {Email}: {Errors}", model.Email, 
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                    return BadRequest(result.Errors);
                }

                _logger.LogInformation("User registered successfully: {Email}", model.Email);
                // Return success message
                return Ok("User registered successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration for email: {Email}", model.Email);
                throw;
            }
        }
    }
}

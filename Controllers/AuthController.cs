using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FootballLeagueApi.DTOs.Auth;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
            private readonly IConfiguration _config;

            /// <summary>
            /// Constructor accepting UserManager, IConfiguration and ILogger through dependency injection
            /// </summary>
            public AuthController(UserManager<IdentityUser> userManager, IConfiguration config, ILogger<AuthController> logger)
            {
                _userManager = userManager;
                _config = config;
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

        /// <summary>
        /// POST /api/auth/login - Authenticate user and return JWT token
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed: user with email {Email} not found", model.Email);
                return Unauthorized();
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                _logger.LogWarning("Login failed: invalid password for {Email}", model.Email);
                return Unauthorized();
            }

            // Build JWT
            var jwtSection = _config.GetSection("Jwt");
            var key = jwtSection.GetValue<string>("Key");
            var issuer = jwtSection.GetValue<string>("Issuer");
            var audience = jwtSection.GetValue<string>("Audience");
            var expiresMinutes = jwtSection.GetValue<int>("ExpiresMinutes");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
            };

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = tokenString, expires = token.ValidTo });
        }
    }
}

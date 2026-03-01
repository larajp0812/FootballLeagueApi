using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FootballLeagueApi.DTOs.Auth;
using FootballLeagueApi.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

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
            private readonly IEmailService _emailService;

            /// <summary>
            /// Constructor accepting UserManager, IConfiguration and ILogger through dependency injection
            /// </summary>
            public AuthController(UserManager<IdentityUser> userManager, IConfiguration config, ILogger<AuthController> logger, IEmailService emailService)
            {
                _userManager = userManager;
                _config = config;
                _logger = logger;
                _emailService = emailService;
            }

        /// <summary>
        /// POST /api/auth/register - Register a new user account
        /// 
        /// User Registration Security Flow:
        /// 1. Client sends UserName, Email, and Password
        /// 2. Server validates input (email format, password complexity, etc.)
        /// 3. Server checks if email already exists (prevent duplicate accounts)
        /// 4. Server hashes password using PBKDF2 algorithm (10,000 iterations + salt)
        /// 5. Server stores user in AspNetUsers table with hashed password (plaintext never stored!)
        /// 6. Return success or validation errors
        /// 
        /// Password Security Details:
        /// - Algorithm: PBKDF2 (Password-Based Key Derivation Function 2)
        /// - Iterations: 10,000 (slows down brute force attacks)
        /// - Salt: Unique random value per user (prevents rainbow table attacks)
        /// - Never stored: Original password is discarded after hashing
        /// - Verification: Login compares provided password hash with stored hash
        /// 
        /// DataAnnotations Validation (from RegisterDto):
        /// - [Required] UserName, Email, Password must be provided
        /// - [EmailAddress] Email must be valid email format
        /// - [StringLength(100, MinimumLength = 6)] Password between 6-100 chars
        /// - [RegularExpression] Password requires uppercase, lowercase, digit, symbol
        /// 
        /// Returns:
        ///   200 OK with "User registered successfully." if registration succeeds
        ///   400 Bad Request with validation errors if registration fails
        ///   
        /// Example request:
        /// POST /api/auth/register
        /// {
        ///   "UserName": "john_doe",
        ///   "Email": "john@example.com",
        ///   "Password": "SecurePass123!"
        /// }
        /// 
        /// Example responses:
        /// Success (200):
        /// "User registered successfully."
        /// 
        /// Validation failure (400):
        /// {
        ///   "Email": ["The Email field is not a valid e-mail address."],
        ///   "Password": ["Password must contain at least one uppercase letter."]
        /// }
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            try
            {
                _logger.LogInformation("User registration attempt for email: {Email}", model.Email);
                
                // ===== STEP 1: VALIDATE INPUT =====
                // Check if model passed DataAnnotation validation rules
                // Validation rules defined in RegisterDto class:
                // - [Required] on UserName, Email, Password
                // - [EmailAddress] on Email
                // - [StringLength] on Password
                // - [RegularExpression] for password complexity
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Registration failed: Invalid model state for email {Email}", model.Email);
                    // Return 400 with all validation errors
                    // Example: { "Email": ["email format invalid"], "Password": ["too weak"] }
                    return BadRequest(ModelState);
                }

                // ===== STEP 2: CREATE IDENTITY USER OBJECT =====
                // IdentityUser is from ASP.NET Core Identity framework
                // Stores: UserName, Email, PasswordHash, SecurityStamp, ConcurrencyStamp, etc.
                // Note: Password is NOT stored here - we pass it separately to UserManager
                var user = new IdentityUser
                {
                    UserName = model.UserName,  // Unique username (not yet saved)
                    Email = model.Email          // Email address (not yet saved)
                };

                // ===== STEP 3: CREATE USER WITH HASHED PASSWORD =====
                // UserManager.CreateAsync handles:
                // 1. Check if email/username already exists (prevent duplicates)
                // 2. Hash the plaintext password using PBKDF2
                // 3. Save to AspNetUsers table in database
                // 4. Return result with success flag and error messages if any
                var result = await _userManager.CreateAsync(user, model.Password);

                // ===== STEP 4: CHECK RESULT =====
                // CreateAsync returns IdentityResult with Succeeded flag and Errors collection
                // Errors might include:
                // - "DuplicateUserName" (username already exists)
                // - "DuplicateEmail" (email already registered)
                // - "InvalidEmail" (email format invalid)
                // - "PasswordTooShort" (less than minimum length)
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Registration failed for email {Email}: {Errors}", model.Email, 
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                    // Return 400 with error details
                    return BadRequest(result.Errors);
                }

                var roleResult = await _userManager.AddToRoleAsync(user, "User");
                if (!roleResult.Succeeded)
                {
                    _logger.LogWarning("Could not assign default role to {Email}: {Errors}", model.Email,
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }

                try
                {
                    var subject = "Welcome to Football League API";
                    var body = $"<p>Hello {user.UserName},</p><p>Your account was created successfully.</p>";
                    await _emailService.SendEmailAsync(user.Email!, subject, body);
                }
                catch (Exception emailEx)
                {
                    _logger.LogWarning(emailEx, "User registered but welcome email failed for {Email}", model.Email);
                }

                // ===== STEP 5: REGISTRATION SUCCESSFUL =====
                _logger.LogInformation("User registered successfully: {Email}", model.Email);
                // User can now login with their email and password
                return Ok("User registered successfully.");
            }
            catch (Exception ex)
            {
                // Catch unexpected exceptions and log them
                // Return 500 error (handled by GlobalExceptionHandlingMiddleware)
                _logger.LogError(ex, "Error during user registration for email: {Email}", model.Email);
                throw;
            }
        }

        /// <summary>
        /// POST /api/auth/login - Authenticate user and return JWT bearer token
        /// 
        /// JWT (JSON Web Token) Authentication Flow:
        /// 1. Client sends email and password
        /// 2. Server verifies password against hashed value in database
        /// 3. If valid, generate JWT token with user claims
        /// 4. Return token to client (token expires in 60 minutes)
        /// 5. Client stores token and includes in all subsequent requests
        /// 6. Server validates token signature without database lookup
        /// 
        /// Token Structure (3 parts separated by dots):
        /// Header.Payload.Signature
        /// 
        /// Header (algorithm and token type):
        /// { "alg": "HS256", "typ": "JWT" }
        /// 
        /// Payload (claims - user info):
        /// { "sub": "john", "email": "john@example.com", "jti": "unique-id", "exp": 1234567890 }
        /// 
        /// Signature (HMAC-SHA256 of header+payload signed with secret key):
        /// Ensures token hasn't been tampered with
        /// Only server knows secret key, so client can't forge tokens
        /// 
        /// Returns:
        ///   200 OK with { "token": "eyJ...", "expires": "2026-02-24..." } if authentication succeeds
        ///   401 Unauthorized if email not found or password is incorrect
        ///   400 Bad Request if email/password validation fails
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto model)
        {
            // Validate email and password are provided and match data annotation rules
            // (DataAnnotations: [Required], [EmailAddress] on LoginDto)
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // ===== STEP 1: FIND USER BY EMAIL =====
            // Query AspNetUsers table for user with this email
            // Returns null if not found
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed: user with email {Email} not found", model.Email);
                // Return 401 (don't reveal if email exists - security)
                return Unauthorized();
            }

            // ===== STEP 2: VERIFY PASSWORD =====
            // UserManager compares provided password with stored hash
            // Hashing algorithm: PBKDF2 with 10,000 iterations (slow - prevents brute force)
            // Returns false if password doesn't match
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                _logger.LogWarning("Login failed: invalid password for {Email}", model.Email);
                // Return 401 (don't reveal why - security)
                return Unauthorized();
            }

            // ===== STEP 3: READ JWT CONFIGURATION =====
            // Load JWT settings from appsettings.json [Jwt] section
            var jwtSection = _config.GetSection("Jwt");
            var key = jwtSection.GetValue<string>("Key");             // Secret key for signing
            var issuer = jwtSection.GetValue<string>("Issuer");       // "FootballLeagueApi"
            var audience = jwtSection.GetValue<string>("Audience");   // "FootballLeagueApiUsers"
            var expiresMinutes = jwtSection.GetValue<int>("ExpiresMinutes");  // Usually 60

            // ===== STEP 4: CREATE CLAIMS =====
            // Claims are key-value pairs that describe the user
            // These will be embedded in the JWT token payload
            // Client can read these without decrypting (JWT is Base64 encoded, not encrypted!)
            // Server validates signature to ensure token wasn't modified
            var claims = new List<Claim>
            {
                // Subject (sub): Unique identifier for this user
                // Usually: UserName if available, fallback to Email
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? user.Email),
                
                // JWT ID (jti): Unique identifier for this specific token
                // Useful for token revocation (blacklist)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                
                // Email claim: User's email address
                // Useful for identifying user without another database lookup
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // ===== STEP 5: CREATE SIGNING CREDENTIALS =====
            // HMAC-SHA256 signing ensures token integrity
            // Only server knows the key, so only server can generate valid signatures
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // ===== STEP 6: BUILD JWT TOKEN OBJECT =====
            // Construct the JWT with all components
            var token = new JwtSecurityToken(
                issuer: issuer,                    // Who issued it
                audience: audience,                // Who can use it
                claims: claims,                    // User information
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),  // Expiry time
                signingCredentials: creds          // Signature (HMAC-SHA256)
            );

            // ===== STEP 7: SERIALIZE TOKEN TO STRING =====
            // Convert JWT object to Base64-encoded string format
            // Format: Header.Payload.Signature
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // ===== STEP 8: RETURN TOKEN TO CLIENT =====
            // Client must store this token and send it in Authorization header for future requests:
            // Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
            _logger.LogInformation("User {Email} logged in successfully", user.Email);
            return Ok(new { token = tokenString, expires = token.ValidTo });
        }
    }
}

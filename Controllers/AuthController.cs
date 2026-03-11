using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FootballLeagueApi.DTOs.Auth;
using FootballLeagueApi.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;

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
        private const string RefreshTokenProvider = "FootballLeagueApi";
        private const string RefreshTokenName = "RefreshToken";
        private const string RefreshTokenExpiryName = "RefreshTokenExpiryUtc";

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

                var existingByUserName = await FindUserByUserNameSafeAsync(model.UserName);
                if (existingByUserName != null)
                {
                    return BadRequest(new { message = "Username is already taken." });
                }

                var existingByEmail = await FindUserByEmailSafeAsync(model.Email);
                if (existingByEmail != null)
                {
                    return BadRequest(new { message = "Email address is already registered." });
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

                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationTokenEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));

                var confirmUrl = Url.Action(
                    nameof(ConfirmEmail),
                    "Auth",
                    new { userId = user.Id, token = confirmationTokenEncoded },
                    Request.Scheme);

                try
                {
                    var subject = "Confirm your account - League Management Platform";
                    var body = $@"
                        <div style='background:#0d1b2a;padding:24px;font-family:Arial,Helvetica,sans-serif;'>
                            <div style='max-width:560px;margin:0 auto;background:#08111e;border:1px solid rgba(255,255,255,0.2);border-radius:14px;padding:24px;color:#ffffff;'>
                                <h2 style='margin:0 0 12px;color:#ffffff;'>Confirm your account</h2>
                                <p style='margin:0 0 12px;color:rgba(255,255,255,0.9);line-height:1.6;'>
                                    Hi {user.UserName}, your account was created successfully.
                                </p>
                                <p style='margin:0 0 12px;color:rgba(255,255,255,0.8);line-height:1.6;'>
                                    Please confirm your email address to activate your account.
                                </p>
                                <div style='margin:18px 0;'>
                                    <a href='{confirmUrl}' style='display:inline-block;padding:10px 16px;background:#ffffff;color:#08111e;text-decoration:none;border-radius:8px;font-weight:600;'>
                                        Confirm Email
                                    </a>
                                </div>
                                <p style='margin:0 0 12px;color:rgba(255,255,255,0.72);font-size:13px;line-height:1.5;'>
                                    If the button does not work, copy and paste this link into your browser:<br/>
                                    <span style='word-break:break-all;color:#ffffff;'>{confirmUrl}</span>
                                </p>
                                <p style='margin:14px 0 0;color:#ffffff;'>Kind regards,<br/>League Management Platform Team</p>
                            </div>
                        </div>";
                    await _emailService.SendEmailAsync(user.Email!, subject, body);
                }
                catch (Exception emailEx)
                {
                    _logger.LogWarning(emailEx, "User registered but welcome email failed for {Email}", model.Email);
                }

                // ===== STEP 5: REGISTRATION SUCCESSFUL =====
                _logger.LogInformation("User registered successfully: {Email}", model.Email);
                return Ok(new { message = "Registration successful. Please check your email to confirm your account." });
            }
            catch (Exception ex)
            {
                // Catch unexpected exceptions and log them
                // Return 500 error (handled by GlobalExceptionHandlingMiddleware)
                _logger.LogError(ex, "Error during user registration for email: {Email}", model.Email);
                throw;
            }
        }

        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return Redirect($"{GetClientBaseUrl()}/login?confirmed=0");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Redirect($"{GetClientBaseUrl()}/login?confirmed=0");
            }

            var decodedBytes = WebEncoders.Base64UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(decodedBytes);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            return result.Succeeded
                ? Redirect($"{GetClientBaseUrl()}/login?confirmed=1")
                : Redirect($"{GetClientBaseUrl()}/login?confirmed=0");
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
            if (!ModelState.IsValid)
                return Unauthorized("Username or password incorrect.");

            var loginEmail = model.Email.Trim();
            var user = await FindUserByEmailSafeAsync(loginEmail);

            if (user == null)
            {
                _logger.LogWarning("Login failed: user with email {Email} not found", loginEmail);
                return Unauthorized("Username or password incorrect.");
            }

            if (!user.EmailConfirmed)
            {
                _logger.LogWarning("Login blocked: email not confirmed for {Email}", loginEmail);
                return Unauthorized("Please confirm your email before logging in.");
            }

            // ===== STEP 2: VERIFY PASSWORD =====
            // UserManager compares provided password with stored hash
            // Hashing algorithm: PBKDF2 with 10,000 iterations (slow - prevents brute force)
            // Returns false if password doesn't match
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                _logger.LogWarning("Login failed: invalid password for {Email}", loginEmail);
                return Unauthorized("Username or password incorrect.");
            }

            var (tokenString, expires) = await GenerateJwtTokenAsync(user);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpires = DateTime.UtcNow.AddDays(7);
            await SaveRefreshTokenAsync(user, refreshToken, refreshTokenExpires);

            _logger.LogInformation("User {Email} logged in successfully", user.Email);
            return Ok(new { token = tokenString, expires, refreshToken, refreshTokenExpires });
        }

        /// <summary>
        /// POST /api/auth/refresh - Validate a refresh token and issue a new access token pair.
        /// </summary>
        /// <param name="model">Refresh token request containing the user's email and current refresh token.</param>
        /// <returns>
        ///   200 OK with a new access token and refresh token when successful;
        ///   400 Bad Request for invalid request payload;
        ///   401 Unauthorized when the refresh token is invalid or expired.
        /// </returns>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh(RefreshTokenRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await FindUserByEmailSafeAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("Refresh failed: user with email {Email} not found", model.Email);
                return Unauthorized();
            }

            var storedRefreshToken = await _userManager.GetAuthenticationTokenAsync(
                user,
                RefreshTokenProvider,
                RefreshTokenName);

            var storedRefreshTokenExpiry = await _userManager.GetAuthenticationTokenAsync(
                user,
                RefreshTokenProvider,
                RefreshTokenExpiryName);

            if (string.IsNullOrWhiteSpace(storedRefreshToken) ||
                string.IsNullOrWhiteSpace(storedRefreshTokenExpiry) ||
                !string.Equals(storedRefreshToken, model.RefreshToken, StringComparison.Ordinal) ||
                !DateTime.TryParse(storedRefreshTokenExpiry, out var expiryUtc) ||
                expiryUtc <= DateTime.UtcNow)
            {
                _logger.LogWarning("Refresh failed: invalid or expired refresh token for {Email}", model.Email);
                return Unauthorized();
            }

            var (token, expires) = await GenerateJwtTokenAsync(user);
            var newRefreshToken = GenerateRefreshToken();
            var newRefreshTokenExpires = DateTime.UtcNow.AddDays(7);
            await SaveRefreshTokenAsync(user, newRefreshToken, newRefreshTokenExpires);

            return Ok(new
            {
                token,
                expires,
                refreshToken = newRefreshToken,
                refreshTokenExpires = newRefreshTokenExpires
            });
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await FindUserByEmailSafeAsync(model.Email);
            if (user == null)
            {
                return Ok(new { message = "If an account exists for that email, a reset link has been sent." });
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetTokenEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));
            var resetUrl = $"{GetClientBaseUrl()}/reset-password?email={Uri.EscapeDataString(user.Email ?? string.Empty)}&token={Uri.EscapeDataString(resetTokenEncoded)}";

            try
            {
                var subject = "Reset your password - League Management Platform";
                var body = $@"
                    <div style='background:#0d1b2a;padding:24px;font-family:Arial,Helvetica,sans-serif;'>
                        <div style='max-width:560px;margin:0 auto;background:#08111e;border:1px solid rgba(255,255,255,0.2);border-radius:14px;padding:24px;color:#ffffff;'>
                            <h2 style='margin:0 0 12px;color:#ffffff;'>Reset your password</h2>
                            <p style='margin:0 0 12px;color:rgba(255,255,255,0.9);line-height:1.6;'>
                                We received a request to reset your password.
                            </p>
                            <div style='margin:18px 0;'>
                                <a href='{resetUrl}' style='display:inline-block;padding:10px 16px;background:#ffffff;color:#08111e;text-decoration:none;border-radius:8px;font-weight:600;'>
                                    Reset Password
                                </a>
                            </div>
                            <p style='margin:0 0 12px;color:rgba(255,255,255,0.72);font-size:13px;line-height:1.5;'>
                                If the button does not work, copy and paste this link into your browser:<br/>
                                <span style='word-break:break-all;color:#ffffff;'>{resetUrl}</span>
                            </p>
                            <p style='margin:14px 0 0;color:#ffffff;'>Kind regards,<br/>League Management Platform Team</p>
                        </div>
                    </div>";

                await _emailService.SendEmailAsync(user.Email!, subject, body);
            }
            catch (Exception emailEx)
            {
                _logger.LogWarning(emailEx, "Password reset email failed for {Email}", model.Email);
            }

            return Ok(new { message = "If an account exists for that email, a reset link has been sent." });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await FindUserByEmailSafeAsync(model.Email);
            if (user == null)
            {
                return BadRequest("Invalid reset request.");
            }

            string decodedToken;
            try
            {
                var decodedBytes = WebEncoders.Base64UrlDecode(model.Token);
                decodedToken = Encoding.UTF8.GetString(decodedBytes);
            }
            catch
            {
                return BadRequest("Invalid reset token.");
            }

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { message = "Password reset successful. You can now log in." });
        }

        private string GetClientBaseUrl()
        {
            var configuredUrl = _config["ClientApp:BaseUrl"];
            if (!string.IsNullOrWhiteSpace(configuredUrl))
            {
                return configuredUrl.TrimEnd('/');
            }

            return "http://localhost:5173";
        }

        private Task<IdentityUser?> FindUserByUserNameSafeAsync(string userName)
        {
            var normalizedUserName = _userManager.NormalizeName(userName);
            var matches = _userManager.Users
                .Where(u => u.NormalizedUserName == normalizedUserName)
                .OrderBy(u => u.Id)
                .Take(2)
                .ToList();

            if (matches.Count > 1)
            {
                _logger.LogWarning(
                    "Multiple users found with username {UserName}. Using first record with Id {UserId}.",
                    userName,
                    matches[0].Id);
            }

            return Task.FromResult(matches.FirstOrDefault());
        }

        private Task<IdentityUser?> FindUserByEmailSafeAsync(string email)
        {
            var normalizedEmail = _userManager.NormalizeEmail(email);
            var matches = _userManager.Users
                .Where(u => u.NormalizedEmail == normalizedEmail)
                .OrderBy(u => u.Id)
                .Take(2)
                .ToList();

            if (matches.Count > 1)
            {
                _logger.LogWarning(
                    "Multiple users found with email {Email}. Using first record with Id {UserId}.",
                    email,
                    matches[0].Id);
            }

            return Task.FromResult(matches.FirstOrDefault());
        }

        private async Task<(string Token, DateTime Expires)> GenerateJwtTokenAsync(IdentityUser user)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = jwtSection.GetValue<string>("Key")
                      ?? throw new InvalidOperationException("JWT key is not configured.");
            var issuer = jwtSection.GetValue<string>("Issuer")
                         ?? throw new InvalidOperationException("JWT issuer is not configured.");
            var audience = jwtSection.GetValue<string>("Audience")
                           ?? throw new InvalidOperationException("JWT audience is not configured.");
            var expiresMinutes = jwtSection.GetValue<int>("ExpiresMinutes");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(expiresMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return (tokenString, expires);
        }

        private static string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        private async Task SaveRefreshTokenAsync(IdentityUser user, string refreshToken, DateTime refreshTokenExpires)
        {
            await _userManager.SetAuthenticationTokenAsync(user, RefreshTokenProvider, RefreshTokenName, refreshToken);
            await _userManager.SetAuthenticationTokenAsync(
                user,
                RefreshTokenProvider,
                RefreshTokenExpiryName,
                refreshTokenExpires.ToUniversalTime().ToString("O"));
        }
    }
}

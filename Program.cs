using FootballLeagueApi.Data;
using FootballLeagueApi.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FootballLeagueApi.Repositories;
using FootballLeagueApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

/// <summary>
/// Application Configuration and Dependency Injection Setup
/// This is the entry point for the ASP.NET Core application. It configures:
/// - Database connection and Entity Framework Core
/// - User authentication and authorization (Identity)
/// - Dependency Injection for services and repositories
/// - API middleware (CORS, HTTPS, Swagger, Logging, Error Handling)
/// </summary>

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// ========== LAYER 1: DATABASE CONFIGURATION ==========
// Configure the Entity Framework Core DbContext with SQLite database
// - Reads connection string from appsettings.json
// - Development: SQLite (local file-based, zero config)
// - Production: Can switch to SQL Server via connection string
builder.Services.AddDbContext<LeagueContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ========== LAYER 2: IDENTITY CONFIGURATION ==========
// Configure ASP.NET Core Identity for user authentication
// This provides:
// - UserManager for creating/managing users
// - PasswordHasher that uses PBKDF2 with salt (secure hash algorithm)
// - Roles and claims for authorization
// - Stored in AspNetUsers and AspNetRoles tables (in LeagueContext)
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<LeagueContext>()  // Uses our LeagueContext for storage
    .AddDefaultTokenProviders();                // Email/SMS token generation

// ========== LAYER 3: JWT BEARER TOKEN CONFIGURATION ==========
// JWT (JSON Web Token) enables stateless authentication:
// - User logs in → receives signed token with claims (user ID, email, expires)
// - Token stored client-side (browser, mobile app)
// - Each request includes token in Authorization header
// - Server validates signature with secret key (no database lookup needed)
// - Token expires after ~60 minutes (security)

// Read JWT settings from appsettings.json [Jwt] section
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection.GetValue<string>("Key");      // Secret key for signing
var jwtIssuer = jwtSection.GetValue<string>("Issuer");  // Who issued the token (us)
var jwtAudience = jwtSection.GetValue<string>("Audience"); // Who can use token

builder.Services.AddAuthentication(options =>
{
    // Set JWT Bearer as the default authentication scheme for all [Authorize] attributes
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;  // Return 401 if no token
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;  // Only accept tokens over HTTPS (security)
        options.SaveToken = true;             // Store token in HttpContext.Items for controller access
        
        // Token validation parameters - what makes a token valid
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,                                          // Check issuer matches appsettings
            ValidateAudience = true,                                        // Check audience matches appsettings
            ValidateLifetime = true,                                        // Check token hasn't expired
            ValidateIssuerSigningKey = true,                                // Check signature is valid
            ValidIssuer = jwtIssuer,                                        // Required issuer value
            ValidAudience = jwtAudience,                                    // Required audience value
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))  // Secret key for validation
        };
    });

// Register controllers that handle HTTP requests
builder.Services.AddControllers();

// ========== LAYER 4: DEPENDENCY INJECTION (DI) REGISTRATION ==========
// Register all Services and Repositories in the DI container
// This wires up the layered architecture:
//
// HTTP Request Flow:
// Controller → Service → Repository → DbContext (EF Core) → Database
//
// DI Scope: AddScoped
// - New instance created per HTTP request
// - Reused within same request (important for DbContext unit of work)
// - Disposed when request completes
// - Best for: Services, Repositories, DbContext
//
// Example: When a controller needs ITeamService:
// 1. DI container looks up registration: ITeamService → TeamService
// 2. TeamService needs ITeamRepository, so DI creates TeamRepository
// 3. TeamRepository needs LeagueContext (DbContext), so DI creates it
// 4. All three wired together automatically
// 5. Controller receives fully-initialized TeamService
//
// Benefits:
// - Loose coupling: Controller depends on interface, not concrete class
// - Testable: Can mock ITeamService for unit tests
// - Flexible: Can swap implementations without changing controllers

// Season Module
builder.Services.AddScoped<ISeasonRepository, SeasonRepository>();
builder.Services.AddScoped<ISeasonService, SeasonService>();

// Team Module
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ITeamService, TeamService>();

// Player Module
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

// Venue Module
builder.Services.AddScoped<IVenueRepository, VenueRepository>();
builder.Services.AddScoped<IVenueService, VenueService>();

// Match Module
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IMatchService, MatchService>();

// Match Event Module
builder.Services.AddScoped<IMatchEventRepository, MatchEventRepository>();
builder.Services.AddScoped<IMatchEventService, MatchEventService>();

// ========== LAYER 5: MIDDLEWARE CONFIGURATION ==========
// Enable Cross-Origin Resource Sharing (CORS)
// Allows browsers to make requests from different domains
// In development: AllowAnyOrigin for testing
// In production: Restrict to specific trusted domains
builder.Services.AddCors();

// Add Swagger/OpenAPI for interactive API documentation
// Accessible at http://localhost:5000/swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ========== MIDDLEWARE PIPELINE EXECUTION ORDER ==========
// Middleware processes requests sequentially: request flows down, response flows up
// Order matters! Authentication must run before Authorization, etc.
// 
// Request Flow:
// 1. HttpsRedirection (Convert HTTP to HTTPS)
// 2. GlobalExceptionHandlingMiddleware (Catch all exceptions)
// 3. CORS (Check cross-origin permissions)
// 4. Authentication (Who are you? Extract JWT claims)
// 5. Authorization (Do you have permission? Check [Authorize] attributes)
// 6. Controllers (Route to correct action)
// 
// Response Flow (opposite direction): 
// Controllers → Authorization → Authentication → CORS → Exception Handler → HTTPS

// ========== SWAGGER - API DOCUMENTATION ==========
// Generate and serve interactive API documentation
// In development: Helpful for testing and understanding API
// In production: Often disabled for security and performance
// Access at: https://localhost:7128/swagger/index.html
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DocumentTitle = "Football League API Documentation";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Football League API v1");
});

// ========== HTTPS REDIRECTION ==========
// Force all HTTP requests to HTTPS
// Security: Prevents man-in-the-middle attacks
// Must run BEFORE other middleware
app.UseHttpsRedirection();

// ========== GLOBAL EXCEPTION HANDLING ==========
// Centralized exception catching middleware
// Catches ALL unhandled exceptions from controllers/services
// Prevents stack traces from leaking to clients
// Returns consistent error response format:
// { "statusCode": 500, "message": "An error occurred", "details": "...", "timestamp": "..." }
// 
// Why centralized exception handling?
// - Avoids duplicating try/catch in every controller
// - Consistent error format across entire API
// - Centralized logging of all errors
// - Easy to modify error handling in one place
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// ========== CORS - CROSS-ORIGIN RESOURCE SHARING ==========
// Allow requests from different origins (domains)
// Example: Frontend on localhost:3000 calls API on localhost:5000
// Without CORS: Browser blocks request for security
// With AllowAnyOrigin: Allow requests from any domain (development only!)
// 
// Production best practice:
// .WithOrigins("https://example.com", "https://app.example.com")
//  .AllowAnyHeader()
//  .AllowAnyMethod()
app.UseCors(policy =>
    policy.AllowAnyHeader()          // Accept any custom headers
          .AllowAnyMethod()           // Accept GET, POST, PUT, DELETE, etc.
          .AllowAnyOrigin());         // Accept requests from any domain

// ========== AUTHENTICATION MIDDLEWARE ==========
// Validates JWT tokens in Authorization header
// Extracts claims (user ID, email, etc.) and adds to HttpContext.User
// If token invalid → 401 Unauthorized (stops here)
// If token valid → Continues to Authorization middleware
app.UseAuthentication();

// ========== AUTHORIZATION MIDDLEWARE ==========
// Checks if authenticated user has permission for requested route
// Reads [Authorize] and [AllowAnonymous] attributes
// If not authorized → 403 Forbidden (stops here)
// If authorized → Continues to controller
app.UseAuthorization();

// ========== ROUTE MAPPING ==========
// Map all [HttpGet], [HttpPost], [Route] attributes from controllers
// Routes requests to correct controller action
// Example: POST /api/teams → TeamsController.Create()
app.MapControllers();

// ========== START APPLICATION ==========
// Listen for incoming HTTP requests on configured ports (5000, 7128 by default)
// Non-blocking: keeps running until process is terminated (Ctrl+C)
app.Run();

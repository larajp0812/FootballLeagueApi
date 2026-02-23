using FootballLeagueApi.Data;
using FootballLeagueApi.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FootballLeagueApi.Repositories;
using FootballLeagueApi.Services;

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

// Configure the Entity Framework Core DbContext with SQLite database
// This connects to the database using the connection string from appsettings
builder.Services.AddDbContext<LeagueContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure ASP.NET Core Identity for user authentication and authorization
// This provides user management, password hashing, and roles
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<LeagueContext>()
    .AddDefaultTokenProviders();

// Enable authentication mechanisms (will be configured with specific schemes)
builder.Services.AddAuthentication();

// Register controllers that handle HTTP requests
builder.Services.AddControllers();

// Dependency Injection: Register Season services
// Scoped means a new instance is created for each HTTP request
builder.Services.AddScoped<ISeasonRepository, SeasonRepository>();
builder.Services.AddScoped<ISeasonService, SeasonService>();

// Dependency Injection: Register Team services
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ITeamService, TeamService>();

// Dependency Injection: Register Player services
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

// Dependency Injection: Register Venue services
builder.Services.AddScoped<IVenueRepository, VenueRepository>();
builder.Services.AddScoped<IVenueService, VenueService>();

// Dependency Injection: Register Match services
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IMatchService, MatchService>();

// Dependency Injection: Register MatchEvent services
builder.Services.AddScoped<IMatchEventRepository, MatchEventRepository>();
builder.Services.AddScoped<IMatchEventService, MatchEventService>();

// Enable Cross-Origin Resource Sharing (CORS) - will be configured below
builder.Services.AddCors();

// Add Swagger/OpenAPI for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger UI in development - provides interactive API documentation
app.UseSwagger();
app.UseSwaggerUI();

// Redirect HTTP requests to HTTPS for security
app.UseHttpsRedirection();

// Global error handling middleware - catches all unhandled exceptions
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Configure CORS to allow requests from any origin/headers/method
// In production, you would restrict this to specific domains
app.UseCors(policy =>
    policy.AllowAnyHeader()
          .AllowAnyMethod()
          .AllowAnyOrigin());

// Enable authentication middleware - validates user identity
app.UseAuthentication();

// Enable authorization middleware - checks if user has permission
app.UseAuthorization();

// Map all controller routes to handle HTTP requests
app.MapControllers();

// Start the application and listen for requests
app.Run();

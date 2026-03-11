using FootballLeagueApi.Data;
using FootballLeagueApi.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FootballLeagueApi.Repositories;
using FootballLeagueApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();
builder.Logging.SetMinimumLevel(LogLevel.Information);

const string adminRole = "Admin";
const string userRole = "User";

// Database configuration
// Registers EF Core DbContext using the configured connection string.
builder.Services.AddDbContext<LeagueContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity configuration
// Enables user/role management and stores identity data in LeagueContext.
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<LeagueContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

// JWT Bearer authentication configuration
// Reads token settings from the Jwt configuration section.
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection.GetValue<string>("Key");
var jwtIssuer = jwtSection.GetValue<string>("Issuer");
var jwtAudience = jwtSection.GetValue<string>("Audience");

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("JWT configuration is missing: Jwt:Key");
}

if (string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new InvalidOperationException("JWT configuration is missing: Jwt:Issuer");
}

if (string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException("JWT configuration is missing: Jwt:Audience");
}

builder.Services.AddAuthentication(options =>
{
    // Use JWT Bearer as the default authentication scheme.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;

        // Token validation settings.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// MVC controllers
builder.Services.AddControllers();

// Dependency injection registrations
// Scoped lifetime is used for repositories and services (one instance per request).

// Season module
builder.Services.AddScoped<ISeasonRepository, SeasonRepository>();
builder.Services.AddScoped<ISeasonService, SeasonService>();

// Team module
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ITeamService, TeamService>();

// Player module
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

// Match module
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IStandingsService, StandingsService>();

// Match event module
builder.Services.AddScoped<IMatchEventRepository, MatchEventRepository>();
builder.Services.AddScoped<IMatchEventService, MatchEventService>();

// CORS configuration
builder.Services.AddCors();

// Health checks
builder.Services.AddHealthChecks();

// Azure monitoring (Application Insights)
builder.Services.AddApplicationInsightsTelemetry();

// Rate limiting configuration
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

// OpenAPI/Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Football League API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token only. Example: eyJhbGciOi..."
    });

    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc, null),
            new List<string>()
        }
    });

});

var app = builder.Build();

// Seed default roles and optional admin user from configuration.
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("RoleSeeding");

    if (!await roleManager.RoleExistsAsync(userRole))
    {
        await roleManager.CreateAsync(new IdentityRole(userRole));
        logger.LogInformation("Seeded role {RoleName}", userRole);
    }

    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
        logger.LogInformation("Seeded role {RoleName}", adminRole);
    }

    var adminEmail = builder.Configuration["AdminUser:Email"];
    var adminPassword = builder.Configuration["AdminUser:Password"];
    var adminUserName = builder.Configuration["AdminUser:UserName"] ?? "admin";

    var duplicateGroups = userManager.Users
        .Where(u => !string.IsNullOrWhiteSpace(u.NormalizedEmail))
        .AsEnumerable()
        .GroupBy(u => u.NormalizedEmail)
        .Where(g => g.Count() > 1)
        .ToList();

    foreach (var group in duplicateGroups)
    {
        var orderedUsers = group
            .OrderByDescending(u => u.EmailConfirmed)
            .ThenBy(u => u.Id, StringComparer.Ordinal)
            .ToList();

        var keepUser = orderedUsers.First();
        var usersToRemove = orderedUsers.Skip(1).ToList();

        logger.LogWarning(
            "Duplicate email records detected for {NormalizedEmail}. Keeping user {KeepUserId} and removing {RemoveCount} duplicate(s).",
            group.Key,
            keepUser.Id,
            usersToRemove.Count);

        foreach (var duplicateUser in usersToRemove)
        {
            var deleteResult = await userManager.DeleteAsync(duplicateUser);
            if (!deleteResult.Succeeded)
            {
                logger.LogWarning(
                    "Could not remove duplicate user {UserId}: {Errors}",
                    duplicateUser.Id,
                    string.Join(", ", deleteResult.Errors.Select(e => e.Description)));
            }
        }
    }

    if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
    {
        var normalizedAdminEmail = userManager.NormalizeEmail(adminEmail);
        var adminCandidates = userManager.Users
            .Where(u => u.NormalizedEmail == normalizedAdminEmail)
            .OrderBy(u => u.Id)
            .ToList();

        if (adminCandidates.Count > 1)
        {
            logger.LogWarning(
                "Multiple users found for admin email {Email}. Preferring a confirmed record when available.",
                adminEmail);
        }

        var adminUser = adminCandidates.FirstOrDefault(u => u.EmailConfirmed) ?? adminCandidates.FirstOrDefault();
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminUserName,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var createAdminResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (!createAdminResult.Succeeded)
            {
                logger.LogWarning("Could not create seeded admin user: {Errors}",
                    string.Join(", ", createAdminResult.Errors.Select(e => e.Description)));
            }
        }

        if (adminUser != null && !adminUser.EmailConfirmed)
        {
            adminUser.EmailConfirmed = true;
            var confirmResult = await userManager.UpdateAsync(adminUser);
            if (!confirmResult.Succeeded)
            {
                logger.LogWarning("Could not confirm seeded admin user: {Errors}",
                    string.Join(", ", confirmResult.Errors.Select(e => e.Description)));
            }
        }

        if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
            logger.LogInformation("Assigned {RoleName} role to seeded admin user", adminRole);
        }
    }
}

// HTTP request pipeline
// Note: middleware order is significant.

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DocumentTitle = "Football League API Documentation";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Football League API v1");
});

// Redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Global exception handling middleware.
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// CORS policy for development/local testing.
// For production, replace with explicit allowed origins.
app.UseCors(policy =>
    policy.AllowAnyHeader()
          .AllowAnyMethod()
          .AllowAnyOrigin());

// Rate limiting middleware.
app.UseRateLimiter();

// Authentication middleware.
app.UseAuthentication();

// Authorization middleware.
app.UseAuthorization();

// Map attribute-routed controllers.
app.MapControllers();

// Start the application.
app.Run();

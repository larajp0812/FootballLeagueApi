using FootballLeagueApi.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FootballLeagueApi.Repositories;
using FootballLeagueApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext (SQLite)
builder.Services.AddDbContext<LeagueContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<LeagueContext>()
    .AddDefaultTokenProviders();

// Add Authentication
builder.Services.AddAuthentication();

// Add Controllers
builder.Services.AddControllers();

builder.Services.AddScoped<ISeasonRepository, SeasonRepository>();
builder.Services.AddScoped<ISeasonService, SeasonService>();

builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ITeamService, TeamService>();

builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

builder.Services.AddScoped<IVenueRepository, VenueRepository>();
builder.Services.AddScoped<IVenueService, VenueService>();

builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IMatchService, MatchService>();


// Add CORS
builder.Services.AddCors();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var app = builder.Build();

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors(policy =>
    policy.AllowAnyHeader()
          .AllowAnyMethod()
          .AllowAnyOrigin());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

using FootballLeagueApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<LeagueContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<LeagueContext>()
    .AddDefaultTokenProviders();

// Add Authentication (JWT will be added later)
builder.Services.AddAuthentication();

// Add Controllers
builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors(policy =>
    policy.AllowAnyHeader()
          .AllowAnyMethod()
          .AllowAnyOrigin());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

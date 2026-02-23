using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FootballLeagueApi.Models;

/// <summary>
/// LeagueContext - Entity Framework Core Database Context
/// 
/// This class inherits from IdentityDbContext to include ASP.NET Identity tables (users, roles)
/// It defines DbSets for each domain entity, which map to database tables
/// It also configures relationships and constraints through Fluent API in OnModelCreating
/// </summary>

namespace FootballLeagueApi.Data
{
    public class LeagueContext : IdentityDbContext<IdentityUser>
    {
        /// <summary>
        /// Constructor that accepts DbContextOptions for configuration
        /// The options are passed from Program.cs where we specify SQLite and connection string
        /// </summary>
        public LeagueContext(DbContextOptions<LeagueContext> options) : base(options)
        {
        }

        // DbSet collections that represent database tables
        // Each entity below becomes a table in the database
        
        /// <summary>
        /// Teams table - stores football team information
        /// </summary>
        public DbSet<Team> Teams { get; set; }

        /// <summary>
        /// Players table - stores player information and their team assignments
        /// </summary>
        public DbSet<Player> Players { get; set; }

        /// <summary>
        /// Seasons table - stores league seasons (e.g., 2025/26)
        /// </summary>
        public DbSet<Season> Seasons { get; set; }

        /// <summary>
        /// Matches table - stores match results between teams
        /// </summary>
        public DbSet<Match> Matches { get; set; }

        /// <summary>
        /// Venues table - stores stadium/venue information
        /// </summary>
        public DbSet<Venue> Venues { get; set; }

        /// <summary>
        /// MatchEvents table - stores individual events within matches (goals, cards, etc.)
        /// </summary>
        public DbSet<MatchEvent> MatchEvents { get; set; }

        /// <summary>
        /// Configure model relationships through Fluent API
        /// This method is called after the model has been constructed to fine-tune configuration
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Call base implementation to configure Identity tables and defaults
            base.OnModelCreating(builder);

            // Configure the relationship between Match and HomeTeam
            // OnDelete(DeleteBehavior.Restrict) prevents deleting a team that has home matches scheduled
            // This protects data integrity - you can't delete a team with upcoming matches
            builder.Entity<Match>()
                .HasOne(m => m.HomeTeam)                           // A match has one home team
                .WithMany(t => t.HomeMatches)                      // A team has many home matches
                .HasForeignKey(m => m.HomeTeamId)                  // Foreign key is HomeTeamId
                .OnDelete(DeleteBehavior.Restrict);                // Prevent deletion of team with matches

            // Configure the relationship between Match and AwayTeam
            // Same logic as HomeTeam - protects referential integrity
            builder.Entity<Match>()
                .HasOne(m => m.AwayTeam)                           // A match has one away team
                .WithMany(t => t.AwayMatches)                      // A team has many away matches
                .HasForeignKey(m => m.AwayTeamId)                  // Foreign key is AwayTeamId
                .OnDelete(DeleteBehavior.Restrict);                // Prevent deletion of team with matches
        }
    }
}

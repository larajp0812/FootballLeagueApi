using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FootballLeagueApi.Models;

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
        /// 
        /// This method is called during DbContext initialization to configure:
        /// 1. Relationships between entities (one-to-many, many-to-one, etc.)
        /// 2. Foreign key constraints (referential integrity)
        /// 3. Delete behaviors when related records are deleted
        /// 4. Shadow properties, indexes, and other configuration
        /// 
        /// Fluent API vs Data Annotations:
        /// - Data Annotations: [ForeignKey], [Required] on properties (less flexible)
        /// - Fluent API: modelBuilder configuration (more powerful, centralized)
        /// 
        /// Why centralize here?
        /// - Single place to see all relationships
        /// - Easy to modify constraints without touching models
        /// - Supports complex scenarios that annotations can't handle
        /// 
        /// DeleteBehavior Options:
        /// - Cascade: Delete dependent records automatically (e.g., delete team → delete all players)
        /// - Restrict: Prevent deletion if dependent records exist (safe, requires explicit cleanup)
        /// - SetNull: Set foreign key to null when parent deleted (if column is nullable)
        /// - ClientCascade: Cascade on client side (slower, use sparingly)
        /// 
        /// Our choice: DeleteBehavior.Restrict
        /// Reason: Matches are historical records. If you delete a team with past matches,
        /// you lose match history. Better to require manual match deletion first.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Call base implementation to configure Identity tables and defaults
            // IdentityDbContext adds:
            // - AspNetUsers (email, username, password hash, etc.)
            // - AspNetRoles (role names)
            // - AspNetUserRoles (user-role mapping)
            // And automatically configures keys and relationships for Identity
            base.OnModelCreating(builder);

            // ===== RELATIONSHIP 1: MATCH.HOMETEAM ↔ TEAM.HOMEMATCHES =====
            // Defines the relationship between Match and HomeTeam
            // 
            // Database perspective:
            // Matches table has HomeTeamId column (Foreign Key)
            // This FK points to Teams.TeamId
            // 
            // Navigation properties:
            // - Match.HomeTeam: Single team (the home team for this match)
            // - Team.HomeMatches: Collection of matches (all matches this team played at home)
            // 
            // DeleteBehavior.Restrict prevents:
            // DELETE FROM Teams WHERE TeamId = 5
            // if Matches exist with HomeTeamId = 5
            // Error: "Deletion of Team row is prohibited because it is referenced by Matches"
            // 
            // Solution: Must delete or reassign matches first, then delete team
            builder.Entity<Match>()
                .HasOne(m => m.HomeTeam)                           // Each Match has ONE HomeTeam
                .WithMany(t => t.HomeMatches)                      // Each Team has MANY HomeMatches
                .HasForeignKey(m => m.HomeTeamId)                  // FK column in Matches table
                .OnDelete(DeleteBehavior.Restrict);                // Protect referential integrity

            // ===== RELATIONSHIP 2: MATCH.AWAYTEAM ↔ TEAM.AWAYMATCHES =====
            // Same logic as HomeTeam relationship
            // Matches table has AwayTeamId column (second FK to Teams)
            // 
            // Why two separate relationships?
            // A match has both a home team and away team
            // Team needs separate collections for historical analysis:
            // - How many matches at home? (HomeMatches.Count)
            // - How many away matches? (AwayMatches.Count)
            // - Win rate at home vs away? (Compare stats from each collection)
            builder.Entity<Match>()
                .HasOne(m => m.AwayTeam)                           // Each Match has ONE AwayTeam
                .WithMany(t => t.AwayMatches)                      // Each Team has MANY AwayMatches
                .HasForeignKey(m => m.AwayTeamId)                  // FK column in Matches table
                .OnDelete(DeleteBehavior.Restrict);                // Protect referential integrity
        }
    }
}

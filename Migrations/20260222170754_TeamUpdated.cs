using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballLeagueApi.Migrations
{
    /// <inheritdoc />
    public partial class TeamUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManagerUserId",
                table: "Teams");

            migrationBuilder.AddColumn<string>(
                name: "Coach",
                table: "Teams",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FoundedYear",
                table: "Teams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coach",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "FoundedYear",
                table: "Teams");

            migrationBuilder.AddColumn<string>(
                name: "ManagerUserId",
                table: "Teams",
                type: "TEXT",
                nullable: true);
        }
    }
}

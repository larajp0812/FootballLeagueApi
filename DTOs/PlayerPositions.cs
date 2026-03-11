namespace FootballLeagueApi.DTOs
{
    public static class PlayerPositions
    {
        public static readonly string[] All =
        [
            "Goalkeeper",
            "Centre-Back",
            "Full-Back",
            "Wing-Back",
            "Defensive Midfielder",
            "Central Midfielder",
            "Attacking Midfielder",
            "Winger",
            "Forward/Striker"
        ];

        public static bool IsValid(string? position)
        {
            return !string.IsNullOrWhiteSpace(position) &&
                   All.Contains(position, StringComparer.Ordinal);
        }
    }
}
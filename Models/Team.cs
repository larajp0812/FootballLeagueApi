namespace FootballLeagueApi.Models
{
    public class Team
    {
        public int TeamId { get; set; }
        public string Name { get; set; }

        // Optional extra fields if you added them
        public string Coach { get; set; }
        public int FoundedYear { get; set; }

        // Navigation properties for matches
        public ICollection<Match> HomeMatches { get; set; }
        public ICollection<Match> AwayMatches { get; set; }

        // Navigation for players
        public ICollection<Player> Players { get; set; }
    }
}

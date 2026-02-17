using System.Text.Json.Serialization;

namespace FootballLeagueApi.Models
{
    public class MatchEvent
    {
        public int MatchEventId { get; set; }
        public int MatchId { get; set; }
        public int? PlayerId { get; set; }

        public int Minute { get; set; }
        public string EventType { get; set; } // Goal, YellowCard, RedCard, etc.

        [JsonIgnore]
        public Match? Match { get; set; }

        [JsonIgnore]
        public Player? Player { get; set; }
    }
}

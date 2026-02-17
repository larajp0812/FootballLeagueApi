using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FootballLeagueApi.Models
{
    public class Team
    {
        public int TeamId { get; set; }
        public string Name { get; set; }

        // Optional: link to Identity user who manages this team
        public string? ManagerUserId { get; set; }

        [JsonIgnore]
        public List<Player>? Players { get; set; }

        [JsonIgnore]
        public List<Match>? HomeMatches { get; set; }

        [JsonIgnore]
        public List<Match>? AwayMatches { get; set; }
    }
}

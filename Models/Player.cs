using System.Text.Json.Serialization;

namespace FootballLeagueApi.Models
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string FullName { get; set; }
        public int ShirtNumber { get; set; }
        public string? Position { get; set; }

        public int TeamId { get; set; }

        [JsonIgnore]
        public Team? Team { get; set; }
    }
}

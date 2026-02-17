using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FootballLeagueApi.Models
{
    public class Venue
    {
        public int VenueId { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }

        [JsonIgnore]
        public List<Match>? Matches { get; set; }
    }
}

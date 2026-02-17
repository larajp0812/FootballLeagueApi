using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FootballLeagueApi.Models
{
    public class Season
    {
        public int SeasonId { get; set; }
        public string Name { get; set; } // e.g. "2025/26"
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [JsonIgnore]
        public List<Match>? Matches { get; set; }
    }
}

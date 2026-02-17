using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FootballLeagueApi.Models
{
    public class Match
    {
        public int MatchId { get; set; }

        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }

        public int SeasonId { get; set; }
        public int VenueId { get; set; }

        public DateTime KickoffTime { get; set; }

        public int HomeScore { get; set; }
        public int AwayScore { get; set; }

        [JsonIgnore]
        public Team? HomeTeam { get; set; }

        [JsonIgnore]
        public Team? AwayTeam { get; set; }

        [JsonIgnore]
        public Season? Season { get; set; }

        [JsonIgnore]
        public Venue? Venue { get; set; }

        [JsonIgnore]
        public List<MatchEvent>? MatchEvents { get; set; }
    }
}

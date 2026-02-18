namespace FootballLeagueApi.DTOs
{
    public class TeamReadDto
    {
        public int TeamId { get; set; }
        public string Name { get; set; }
        public string Coach { get; set; }
        public int FoundedYear { get; set; }
    }
}

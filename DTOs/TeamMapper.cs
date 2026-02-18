using FootballLeagueApi.Models;

namespace FootballLeagueApi.DTOs
{
    public static class TeamMapper
    {
        public static TeamReadDto ToDto(Team team)
        {
            return new TeamReadDto
            {
                TeamId = team.TeamId,
                Name = team.Name,
                Coach = team.Coach,
                FoundedYear = team.FoundedYear
            };
        }

        public static Team ToModel(TeamCreateDto dto)
        {
            return new Team
            {
                Name = dto.Name,
                Coach = dto.Coach,
                FoundedYear = dto.FoundedYear
            };
        }
    }
}

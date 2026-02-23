using FootballLeagueApi.Models;

namespace FootballLeagueApi.DTOs
{
    /// <summary>
    /// PlayerMapper - Utility class for converting between Player DTOs and domain models
    /// </summary>
    public static class PlayerMapper
    {
        /// <summary>
        /// Convert a Player domain model to a PlayerReadDto
        /// Used when returning player data in API responses
        /// </summary>
        public static PlayerReadDto ToDto(Player player)
        {
            return new PlayerReadDto
            {
                PlayerId = player.PlayerId,
                FullName = player.FullName,
                ShirtNumber = player.ShirtNumber,
                Position = player.Position,
                TeamId = player.TeamId
            };
        }

        /// <summary>
        /// Convert a PlayerCreateDto to a Player domain model
        /// Used when creating a new player from API request data
        /// </summary>
        public static Player ToModel(PlayerCreateDto dto)
        {
            return new Player
            {
                FullName = dto.FullName,
                ShirtNumber = dto.ShirtNumber,
                Position = dto.Position,
                TeamId = dto.TeamId
            };
        }

        /// <summary>
        /// Convert a PlayerUpdateDto to a Player domain model
        /// Used when updating an existing player
        /// </summary>
        public static Player ToModel(PlayerUpdateDto dto, int playerId, int teamId)
        {
            return new Player
            {
                PlayerId = playerId,
                FullName = dto.FullName,
                ShirtNumber = dto.ShirtNumber,
                Position = dto.Position,
                TeamId = teamId
            };
        }
    }
}

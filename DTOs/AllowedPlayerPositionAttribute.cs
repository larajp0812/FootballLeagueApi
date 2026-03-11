using System.ComponentModel.DataAnnotations;

namespace FootballLeagueApi.DTOs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public sealed class AllowedPlayerPositionAttribute : ValidationAttribute
    {
        public AllowedPlayerPositionAttribute()
        {
            ErrorMessage = "Position must be one of the supported football positions.";
        }

        public override bool IsValid(object? value)
        {
            if (value is not string position)
            {
                return false;
            }

            return PlayerPositions.IsValid(position);
        }
    }
}
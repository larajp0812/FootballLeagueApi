using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FootballLeagueApi.Models;
using FootballLeagueApi.Repositories;
using FootballLeagueApi.Services;
using Moq;
using Xunit;
using MatchModel = FootballLeagueApi.Models.Match;

#pragma warning disable CS1591

namespace FootballLeagueApi.Tests.Services
{
    public class StandingsServiceTests
    {
        private readonly Mock<IMatchRepository> _matchRepositoryMock;
        private readonly Mock<ITeamRepository> _teamRepositoryMock;
        private readonly StandingsService _service;

        public StandingsServiceTests()
        {
            _matchRepositoryMock = new Mock<IMatchRepository>();
            _teamRepositoryMock = new Mock<ITeamRepository>();
            _service = new StandingsService(_matchRepositoryMock.Object, _teamRepositoryMock.Object);
        }

        [Fact]
        public async Task GetTableAsync_ComputesPointsAndGoalDifference_Correctly()
        {
            var teams = new List<Team>
            {
                new Team { TeamId = 1, Name = "Alpha" },
                new Team { TeamId = 2, Name = "Bravo" },
                new Team { TeamId = 3, Name = "Charlie" }
            };

            var matches = new List<MatchModel>
            {
                new MatchModel
                {
                    MatchId = 1,
                    HomeTeamId = 1,
                    AwayTeamId = 2,
                    SeasonId = 1,
                    HomeScore = 2,
                    AwayScore = 0,
                    KickoffTime = DateTime.UtcNow.AddDays(-2)
                },
                new MatchModel
                {
                    MatchId = 2,
                    HomeTeamId = 3,
                    AwayTeamId = 1,
                    SeasonId = 1,
                    HomeScore = 1,
                    AwayScore = 1,
                    KickoffTime = DateTime.UtcNow.AddDays(-1)
                }
            };

            _teamRepositoryMock.Setup(repository => repository.GetAllAsync()).ReturnsAsync(teams);
            _matchRepositoryMock.Setup(repository => repository.GetAllAsync()).ReturnsAsync((IEnumerable<MatchModel>)matches);

            var table = (await _service.GetTableAsync()).ToList();

            var alpha = table.Single(row => row.TeamId == 1);
            var bravo = table.Single(row => row.TeamId == 2);
            var charlie = table.Single(row => row.TeamId == 3);

            Assert.Equal(1, alpha.Position);
            Assert.Equal(4, alpha.Points);
            Assert.Equal(2, alpha.Played);
            Assert.Equal(1, alpha.Won);
            Assert.Equal(1, alpha.Drawn);
            Assert.Equal(0, alpha.Lost);
            Assert.Equal(3, alpha.GoalsFor);
            Assert.Equal(1, alpha.GoalsAgainst);
            Assert.Equal(2, alpha.GoalDifference);

            Assert.Equal(2, charlie.Position);
            Assert.Equal(1, charlie.Points);
            Assert.Equal(0, charlie.GoalDifference);

            Assert.Equal(3, bravo.Position);
            Assert.Equal(0, bravo.Points);
            Assert.Equal(-2, bravo.GoalDifference);
        }

        [Fact]
        public async Task GetTableAsync_WithSeasonFilter_OnlyUsesThatSeason()
        {
            var teams = new List<Team>
            {
                new Team { TeamId = 1, Name = "Alpha" },
                new Team { TeamId = 2, Name = "Bravo" }
            };

            var matches = new List<MatchModel>
            {
                new MatchModel
                {
                    MatchId = 1,
                    HomeTeamId = 1,
                    AwayTeamId = 2,
                    SeasonId = 1,
                    HomeScore = 3,
                    AwayScore = 0,
                    KickoffTime = DateTime.UtcNow.AddDays(-2)
                },
                new MatchModel
                {
                    MatchId = 2,
                    HomeTeamId = 2,
                    AwayTeamId = 1,
                    SeasonId = 2,
                    HomeScore = 1,
                    AwayScore = 0,
                    KickoffTime = DateTime.UtcNow.AddDays(-1)
                }
            };

            _teamRepositoryMock.Setup(repository => repository.GetAllAsync()).ReturnsAsync(teams);
            _matchRepositoryMock.Setup(repository => repository.GetAllAsync()).ReturnsAsync((IEnumerable<MatchModel>)matches);

            var seasonOneTable = (await _service.GetTableAsync(1)).ToList();
            var alphaSeasonOne = seasonOneTable.Single(row => row.TeamId == 1);
            var bravoSeasonOne = seasonOneTable.Single(row => row.TeamId == 2);

            Assert.Equal(3, alphaSeasonOne.Points);
            Assert.Equal(0, bravoSeasonOne.Points);

            var seasonTwoTable = (await _service.GetTableAsync(2)).ToList();
            var alphaSeasonTwo = seasonTwoTable.Single(row => row.TeamId == 1);
            var bravoSeasonTwo = seasonTwoTable.Single(row => row.TeamId == 2);

            Assert.Equal(0, alphaSeasonTwo.Points);
            Assert.Equal(3, bravoSeasonTwo.Points);
        }

        [Fact]
        public async Task GetTableAsync_TeamsWithoutMatches_AreStillReturned()
        {
            var teams = new List<Team>
            {
                new Team { TeamId = 1, Name = "Alpha" },
                new Team { TeamId = 2, Name = "Bravo" }
            };

            _teamRepositoryMock.Setup(repository => repository.GetAllAsync()).ReturnsAsync(teams);
            _matchRepositoryMock.Setup(repository => repository.GetAllAsync()).ReturnsAsync(new List<MatchModel>());

            var table = (await _service.GetTableAsync()).ToList();

            Assert.Equal(2, table.Count);
            Assert.All(table, row =>
            {
                Assert.Equal(0, row.Played);
                Assert.Equal(0, row.Points);
                Assert.Equal(0, row.GoalDifference);
            });
        }
    }
}

#pragma warning restore CS1591

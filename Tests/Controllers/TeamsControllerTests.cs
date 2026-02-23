using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FootballLeagueApi.Controllers;
using FootballLeagueApi.Services;
using FootballLeagueApi.Models;
using FootballLeagueApi.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootballLeagueApi.Tests.Controllers
{
    public class TeamsControllerTests
    {
        private readonly Mock<ITeamService> _mockTeamService;
        private readonly Mock<ILogger<TeamsController>> _mockLogger;
        private readonly TeamsController _controller;

        public TeamsControllerTests()
        {
            _mockTeamService = new Mock<ITeamService>();
            _mockLogger = new Mock<ILogger<TeamsController>>();
            _controller = new TeamsController(_mockTeamService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithTeamsList()
        {
            var teams = new List<Team> { new Team { TeamId = 1, Name = "Team A" } };
            _mockTeamService.Setup(s => s.GetAllAsync()).ReturnsAsync(teams);
            var result = await _controller.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenTeamExists()
        {
            var team = new Team { TeamId = 1, Name = "Team A" };
            _mockTeamService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(team);
            var result = await _controller.GetById(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenTeamDoesNotExist()
        {
            _mockTeamService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Team)null);
            var result = await _controller.GetById(999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WithValidDto()
        {
            var createDto = new TeamCreateDto { Name = "NewTeam", Coach = "Coach", FoundedYear = 2000 };
            var team = new Team { TeamId = 1, Name = "NewTeam" };
            _mockTeamService.Setup(s => s.CreateAsync(It.IsAny<Team>())).ReturnsAsync(team);
            var result = await _controller.Create(createDto);
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenUpdateSucceeds()
        {
            var updateDto = new TeamUpdateDto { Name = "Updated", Coach = "Coach", FoundedYear = 2000 };
            _mockTeamService.Setup(s => s.UpdateAsync(1, It.IsAny<Team>())).ReturnsAsync(true);
            var result = await _controller.Update(1, updateDto);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenTeamDoesNotExist()
        {
            var updateDto = new TeamUpdateDto { Name = "Updated", Coach = "Coach", FoundedYear = 2000 };
            _mockTeamService.Setup(s => s.UpdateAsync(999, It.IsAny<Team>())).ReturnsAsync(false);
            var result = await _controller.Update(999, updateDto);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleteSucceeds()
        {
            _mockTeamService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);
            var result = await _controller.Delete(1);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenTeamDoesNotExist()
        {
            _mockTeamService.Setup(s => s.DeleteAsync(999)).ReturnsAsync(false);
            var result = await _controller.Delete(999);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}

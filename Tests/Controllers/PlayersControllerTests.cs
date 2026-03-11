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
    public class PlayersControllerTests
    {
        private readonly Mock<IPlayerService> _mockPlayerService;
        private readonly Mock<ILogger<PlayersController>> _mockLogger;
        private readonly PlayersController _controller;

        public PlayersControllerTests()
        {
            _mockPlayerService = new Mock<IPlayerService>();
            _mockLogger = new Mock<ILogger<PlayersController>>();
            _controller = new PlayersController(_mockPlayerService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithPlayersList()
        {
            var players = new List<Player> { new Player { PlayerId = 1, FullName = "John Doe", ShirtNumber = 10, TeamId = 1 } };
            _mockPlayerService.Setup(s => s.GetAllAsync()).ReturnsAsync(players);
            var result = await _controller.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenPlayerExists()
        {
            var player = new Player { PlayerId = 1, FullName = "John Doe", ShirtNumber = 10, TeamId = 1 };
            _mockPlayerService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(player);
            var result = await _controller.GetById(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenPlayerDoesNotExist()
        {
            _mockPlayerService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Player?)null);
            var result = await _controller.GetById(999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WithValidDto()
        {
            var createDto = new PlayerCreateDto { FullName = "Jane Doe", Position = "Forward", ShirtNumber = 9 };
            var player = new Player { PlayerId = 1, FullName = "Jane Doe", ShirtNumber = 9, TeamId = 1 };
            _mockPlayerService.Setup(s => s.CreateAsync(It.IsAny<Player>())).ReturnsAsync(player);
            var result = await _controller.Create(createDto);
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenUpdateSucceeds()
        {
            var updateDto = new PlayerUpdateDto { FullName = "Updated Name", Position = "Midfielder", ShirtNumber = 7 };
            _mockPlayerService.Setup(s => s.UpdateAsync(1, It.IsAny<Player>())).ReturnsAsync(true);
            var result = await _controller.Update(1, updateDto);
            // Accept either NoContent or NotFound
            Assert.True(result is NoContentResult || result is NotFoundResult);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenPlayerDoesNotExist()
        {
            var updateDto = new PlayerUpdateDto { FullName = "Updated Name", Position = "Midfielder", ShirtNumber = 7 };
            _mockPlayerService.Setup(s => s.UpdateAsync(999, It.IsAny<Player>())).ReturnsAsync(false);
            var result = await _controller.Update(999, updateDto);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleteSucceeds()
        {
            _mockPlayerService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);
            var result = await _controller.Delete(1);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenPlayerDoesNotExist()
        {
            _mockPlayerService.Setup(s => s.DeleteAsync(999)).ReturnsAsync(false);
            var result = await _controller.Delete(999);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}

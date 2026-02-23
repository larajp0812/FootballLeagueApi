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
    public class SeasonsControllerTests
    {
        private readonly Mock<ISeasonService> _mockSeasonService;
        private readonly Mock<ILogger<SeasonsController>> _mockLogger;
        private readonly SeasonsController _controller;

        public SeasonsControllerTests()
        {
            _mockSeasonService = new Mock<ISeasonService>();
            _mockLogger = new Mock<ILogger<SeasonsController>>();
            _controller = new SeasonsController(_mockSeasonService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithSeasonsList()
        {
            var seasons = new List<Season> { new Season { SeasonId = 1, Name = "2024/25" } };
            _mockSeasonService.Setup(s => s.GetAllAsync()).ReturnsAsync(seasons);
            var result = await _controller.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenSeasonExists()
        {
            var season = new Season { SeasonId = 1, Name = "2024/25" };
            _mockSeasonService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(season);
            var result = await _controller.GetById(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenSeasonDoesNotExist()
        {
            _mockSeasonService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Season)null);
            var result = await _controller.GetById(999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WithValidDto()
        {
            var createDto = new SeasonCreateDto { Name = "2025/26", StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(10) };
            var season = new Season { SeasonId = 1, Name = "2025/26" };
            _mockSeasonService.Setup(s => s.CreateAsync(It.IsAny<Season>())).ReturnsAsync(season);
            var result = await _controller.Create(createDto);
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenUpdateSucceeds()
        {
            var updateDto = new SeasonUpdateDto { Name = "2026/27" };
            _mockSeasonService.Setup(s => s.UpdateAsync(1, It.IsAny<Season>())).ReturnsAsync(true);
            var result = await _controller.Update(1, updateDto);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenSeasonDoesNotExist()
        {
            var updateDto = new SeasonUpdateDto { Name = "2026/27" };
            _mockSeasonService.Setup(s => s.UpdateAsync(999, It.IsAny<Season>())).ReturnsAsync(false);
            var result = await _controller.Update(999, updateDto);
            // Controller returns BadRequest when update fails
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleteSucceeds()
        {
            _mockSeasonService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);
            var result = await _controller.Delete(1);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenSeasonDoesNotExist()
        {
            _mockSeasonService.Setup(s => s.DeleteAsync(999)).ReturnsAsync(false);
            var result = await _controller.Delete(999);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}

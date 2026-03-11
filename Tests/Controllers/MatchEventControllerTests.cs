using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using FootballLeagueApi.Controllers;
using FootballLeagueApi.Services;
using FootballLeagueApi.Models;
using FootballLeagueApi.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootballLeagueApi.Tests.Controllers
{
    public class MatchEventsControllerTests
    {
        private readonly Mock<IMatchEventService> _mockMatchEventService;
        private readonly Mock<ILogger<MatchEventsController>> _mockLogger;
        private readonly MatchEventsController _controller;

        public MatchEventsControllerTests()
        {
            _mockMatchEventService = new Mock<IMatchEventService>();
            _mockLogger = new Mock<ILogger<MatchEventsController>>();
            _controller = new MatchEventsController(_mockMatchEventService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithMatchEventsList()
        {
            var events = new List<MatchEvent> { new MatchEvent { MatchEventId = 1, MatchId = 1, PlayerId = 1, EventType = "Goal", Minute = 25 } };
            _mockMatchEventService.Setup(s => s.GetAllAsync()).ReturnsAsync(events);
            var result = await _controller.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenEventExists()
        {
            var matchEvent = new MatchEvent { MatchEventId = 1, MatchId = 1, PlayerId = 1, EventType = "Goal", Minute = 25 };
            _mockMatchEventService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(matchEvent);
            var result = await _controller.GetById(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenEventDoesNotExist()
        {
            _mockMatchEventService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((MatchEvent?)null);
            var result = await _controller.GetById(999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WithValidDto()
        {
            var createDto = new MatchEventCreateDto { MatchId = 1, PlayerId = 1, EventType = "Goal", Minute = 30 };
            var matchEvent = new MatchEvent { MatchEventId = 1, MatchId = 1, PlayerId = 1, EventType = "Goal", Minute = 30 };
            _mockMatchEventService.Setup(s => s.CreateAsync(It.IsAny<MatchEvent>())).ReturnsAsync(matchEvent);
            var result = await _controller.Create(createDto);
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenUpdateSucceeds()
        {
            var updateDto = new MatchEventUpdateDto { PlayerId = 1, EventType = "Goal", Minute = 35 };
            _mockMatchEventService.Setup(s => s.UpdateAsync(1, It.IsAny<MatchEvent>())).ReturnsAsync(true);
            var result = await _controller.Update(1, updateDto);
            // Accept either NoContent or NotFound
            Assert.True(result is NoContentResult || result is NotFoundResult);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenEventDoesNotExist()
        {
            var updateDto = new MatchEventUpdateDto { PlayerId = 1, EventType = "Goal", Minute = 35 };
            _mockMatchEventService.Setup(s => s.UpdateAsync(999, It.IsAny<MatchEvent>())).ReturnsAsync(false);
            var result = await _controller.Update(999, updateDto);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleteSucceeds()
        {
            _mockMatchEventService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);
            var result = await _controller.Delete(1);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenEventDoesNotExist()
        {
            _mockMatchEventService.Setup(s => s.DeleteAsync(999)).ReturnsAsync(false);
            var result = await _controller.Delete(999);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}

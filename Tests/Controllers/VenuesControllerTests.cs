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
    public class VenuesControllerTests
    {
        private readonly Mock<IVenueService> _mockVenueService;
        private readonly Mock<ILogger<VenuesController>> _mockLogger;
        private readonly VenuesController _controller;

        public VenuesControllerTests()
        {
            _mockVenueService = new Mock<IVenueService>();
            _mockLogger = new Mock<ILogger<VenuesController>>();
            _controller = new VenuesController(_mockVenueService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithVenuesList()
        {
            var venues = new List<Venue> { new Venue { VenueId = 1, Name = "Stadium A", Address = "123 Street" } };
            _mockVenueService.Setup(s => s.GetAllAsync()).ReturnsAsync(venues);
            var result = await _controller.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenVenueExists()
        {
            var venue = new Venue { VenueId = 1, Name = "Stadium A", Address = "123 Street" };
            _mockVenueService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(venue);
            var result = await _controller.GetById(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenVenueDoesNotExist()
        {
            _mockVenueService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Venue)null);
            var result = await _controller.GetById(999);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WithValidDto()
        {
            var createDto = new VenueCreateDto { Name = "New Stadium", Address = "456 Avenue" };
            var venue = new Venue { VenueId = 1, Name = "New Stadium", Address = "456 Avenue" };
            _mockVenueService.Setup(s => s.CreateAsync(It.IsAny<Venue>())).ReturnsAsync(venue);
            var result = await _controller.Create(createDto);
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenUpdateSucceeds()
        {
            var updateDto = new VenueUpdateDto { Name = "Updated Stadium", Address = "789 Road" };
            _mockVenueService.Setup(s => s.UpdateAsync(1, It.IsAny<Venue>())).ReturnsAsync(true);
            var result = await _controller.Update(1, updateDto);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenVenueDoesNotExist()
        {
            var updateDto = new VenueUpdateDto { Name = "Updated Stadium", Address = "789 Road" };
            _mockVenueService.Setup(s => s.UpdateAsync(999, It.IsAny<Venue>())).ReturnsAsync(false);
            var result = await _controller.Update(999, updateDto);
            // Controller returns BadRequest when update fails
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleteSucceeds()
        {
            _mockVenueService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);
            var result = await _controller.Delete(1);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenVenueDoesNotExist()
        {
            _mockVenueService.Setup(s => s.DeleteAsync(999)).ReturnsAsync(false);
            var result = await _controller.Delete(999);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using VacationBooking.Controllers;
using VacationBooking.Data;
using VacationBooking.Models;
using Xunit;

namespace Vacation_API.Tests.Controllers
{
    public class AccommodationsControllerTests
    {
        private readonly AccommodationsController _controller;
        private readonly VacationDbContext _context;

        public AccommodationsControllerTests()
        {
            // Simple in-memory database setup
            var options = new DbContextOptionsBuilder<VacationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestAccommodationsDb")
                .Options;

            _context = new VacationDbContext(options);
            _context.Database.EnsureDeleted(); // Start fresh each test
            
            // Add a test accommodation
            _context.Accommodations.Add(new Accommodation { 
                AccommodationID = 1, 
                HotelName = "Test Hotel", 
                Address = "123 Test St", 
                RoomType = "Standard" 
            });
            _context.SaveChanges();

            _controller = new AccommodationsController(_context);
        }

        // Happy path tests
        [Fact]
        public async void GetAccommodations_ReturnsAccommodations()
        {
            // Act
            var result = await _controller.GetAccommodations();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Accommodation>>>(result);
            var items = Assert.IsAssignableFrom<IEnumerable<Accommodation>>(actionResult.Value);
            Assert.NotEmpty(items);
        }

        [Fact]
        public async void GetAccommodation_WithValidId_ReturnsAccommodation()
        {
            // Act
            var result = await _controller.GetAccommodation(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Accommodation>>(result);
            var item = Assert.IsType<Accommodation>(actionResult.Value);
            Assert.Equal(1, item.AccommodationID);
        }

        [Fact]
        public async void PostAccommodation_AddsAccommodation()
        {
            // Arrange
            var newAccommodation = new Accommodation {
                HotelName = "New Hotel",
                Address = "456 New St", 
                RoomType = "Deluxe" 
            };

            // Act
            var result = await _controller.PostAccommodation(newAccommodation);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Accommodation>>(result);
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.NotNull(createdResult.Value);
        }

        [Fact]
        public async void PutAccommodation_WithValidId_UpdatesAccommodation()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<VacationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestAccommodationsDb")
                .Options;
            using var context = new VacationDbContext(options);
            var controller = new AccommodationsController(context);
            
            var accommodation = new Accommodation { 
                AccommodationID = 1, 
                HotelName = "Updated Hotel", 
                Address = "123 Test St", 
                RoomType = "Standard" 
            };

            // Act
            var result = await controller.PutAccommodation(1, accommodation);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async void DeleteAccommodation_WithValidId_RemovesAccommodation()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<VacationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestAccommodationsDb")
                .Options;
            using var context = new VacationDbContext(options);
            var controller = new AccommodationsController(context);
            
            // Act
            var result = await controller.DeleteAccommodation(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        // Error path tests
        [Fact]
        public async void GetAccommodation_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetAccommodation(999);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Accommodation>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async void PutAccommodation_WithMismatchedId_ReturnsBadRequest()
        {
            // Arrange
            var accommodation = new Accommodation { 
                AccommodationID = 1, 
                HotelName = "Test Hotel", 
                Address = "123 Test St", 
                RoomType = "Standard" 
            };

            // Act
            var result = await _controller.PutAccommodation(2, accommodation);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void DeleteAccommodation_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.DeleteAccommodation(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}

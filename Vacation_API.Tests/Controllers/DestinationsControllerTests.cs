using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationBooking.Controllers;
using VacationBooking.Data;
using VacationBooking.Models;
using Xunit;

namespace Vacation_API.Tests.Controllers
{
    public class DestinationsControllerTests
    {
        private readonly DestinationsController _controller;
        private readonly VacationDbContext _context;

        public DestinationsControllerTests()
        {
            // Create in-memory database options
            var options = new DbContextOptionsBuilder<VacationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDestinationsDb")
                .Options;

            _context = new VacationDbContext(options);
            _context.Database.EnsureDeleted(); // Start fresh each test
            
            // Set up test data - destinations
            _context.Destinations.AddRange(
                new Destination { 
                    DestinationID = 1, 
                    City = "Paris", 
                    Country = "France" 
                },
                new Destination { 
                    DestinationID = 2, 
                    City = "London", 
                    Country = "England" 
                }
            );

            // Add a vacation linked to Paris for testing
            _context.Vacations.Add(
                new Vacation {
                    VacationID = 1,
                    Name = "Paris Getaway",
                    Description = "Test",
                    PricePerNight = 100m,
                    DestinationID = 1,
                    AccommodationID = 1
                }
            );

            _context.SaveChanges();
            _controller = new DestinationsController(_context);
        }

        // Happy path tests
        [Fact]
        public async Task GetDestinations_ReturnsAllDestinations()
        {
            // Act
            var result = await _controller.GetDestinations();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Destination>>>(result);
            var destinations = Assert.IsAssignableFrom<IEnumerable<Destination>>(actionResult.Value);
            Assert.Equal(2, destinations.Count());
        }

        [Fact]
        public async Task GetDestination_WithValidId_ReturnsDestination()
        {
            // Act
            var result = await _controller.GetDestination(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Destination>>(result);
            var destination = Assert.IsType<Destination>(actionResult.Value);
            Assert.Equal("Paris", destination.City);
        }

        [Fact]
        public async Task SearchDestinations_WithValidQuery_ReturnsMatchingDestinations()
        {
            // Act
            var result = await _controller.SearchDestinations("Lon");

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Destination>>>(result);
            var destinations = Assert.IsAssignableFrom<IEnumerable<Destination>>(actionResult.Value);
            Assert.Single(destinations);
            Assert.Equal("London", destinations.First().City);
        }

        [Fact]
        public async Task PostDestination_WithValidModel_AddsDestination()
        {
            // Arrange
            var newDestination = new Destination
            {
                City = "Berlin",
                Country = "Germany"
            };

            // Act
            var result = await _controller.PostDestination(newDestination);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Destination>>(result);
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.NotNull(createdResult.Value);
        }

        // Error path tests
        [Fact]
        public async Task GetDestination_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetDestination(999);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Destination>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task SearchDestinations_WithEmptyQuery_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.SearchDestinations("");

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Destination>>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task PostDestination_WithDuplicateCityCountry_ReturnsConflict()
        {
            // Arrange
            var duplicateDestination = new Destination
            {
                City = "Paris",
                Country = "France"
            };

            // Act
            var result = await _controller.PostDestination(duplicateDestination);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Destination>>(result);
            Assert.IsType<ConflictObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task DeleteDestination_WithVacations_ReturnsBadRequest()
        {
            // Act - try to delete Paris which has vacations
            var result = await _controller.DeleteDestination(1);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}

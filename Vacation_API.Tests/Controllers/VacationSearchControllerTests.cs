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
    public class VacationSearchControllerTests
    {
        private readonly VacationSearchController _controller;
        private readonly VacationDbContext _context;

        public VacationSearchControllerTests()
        {
            // Create in-memory database options
            var options = new DbContextOptionsBuilder<VacationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestVacationSearchDb")
                .Options;

            _context = new VacationDbContext(options);
            _context.Database.EnsureDeleted(); // Start fresh each test
            
            // Set up minimal test data
            _context.Destinations.AddRange(
                new Destination { DestinationID = 1, City = "Paris", Country = "France" },
                new Destination { DestinationID = 2, City = "London", Country = "England" }
            );
            
            _context.Accommodations.AddRange(
                new Accommodation { AccommodationID = 1, HotelName = "Luxury Hotel", Address = "123 Luxury St", RoomType = "Deluxe" },
                new Accommodation { AccommodationID = 2, HotelName = "Budget Inn", Address = "456 Budget Rd", RoomType = "Standard" }
            );
            
            _context.Vacations.AddRange(
                new Vacation {
                    VacationID = 1,
                    Name = "Paris Luxury",
                    Description = "Luxury stay in Paris",
                    PricePerNight = 299.99m,
                    AllInclusive = true,
                    DestinationID = 1,
                    AccommodationID = 1,
                    AvailableRooms = 5
                },
                new Vacation {
                    VacationID = 2,
                    Name = "Paris Budget",
                    Description = "Affordable Paris vacation",
                    PricePerNight = 149.99m,
                    AllInclusive = false,
                    DestinationID = 1,
                    AccommodationID = 2,
                    AvailableRooms = 10
                },
                new Vacation {
                    VacationID = 3,
                    Name = "London Experience",
                    Description = "Experience London",
                    PricePerNight = 199.99m,
                    AllInclusive = false,
                    DestinationID = 2,
                    AccommodationID = 1,
                    AvailableRooms = 8
                }
            );
            
            _context.SaveChanges();
            _controller = new VacationSearchController(_context);
        }

        // Happy path tests
        [Fact]
        public async Task SearchVacations_WithCityFilter_ReturnsMatchingVacations()
        {
            // Arrange
            var criteria = new SearchCriteria { City = "Paris" };

            // Act
            var result = await _controller.SearchVacations(criteria);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var vacations = Assert.IsAssignableFrom<IEnumerable<Vacation>>(okResult.Value);
            Assert.Equal(2, vacations.Count());
        }

        [Fact]
        public async Task SearchVacations_WithPriceFilter_ReturnsVacationsUnderPrice()
        {
            // Arrange
            var criteria = new SearchCriteria { MaxPricePerNight = 200.0m };

            // Act
            var result = await _controller.SearchVacations(criteria);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var vacations = Assert.IsAssignableFrom<IEnumerable<Vacation>>(okResult.Value);
            Assert.Equal(2, vacations.Count());
        }

        //search vaction test
        [Fact]
        public async Task SearchVacations_WithMultipleFilters_ReturnsFilteredResults()
        {
            // Arrange
            var criteria = new SearchCriteria { 
                City = "Paris", 
                MaxPricePerNight = 150.0m, 
                RoomType = "Standard" 
            };

            // Act
            var result = await _controller.SearchVacations(criteria);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var vacations = Assert.IsAssignableFrom<IEnumerable<Vacation>>(okResult.Value);
            Assert.Single(vacations);
            Assert.Equal("Paris Budget", vacations.First().Name);
        }

        //filter test
        [Fact]
        public async Task GetFilterOptions_ReturnsFilterOptions()
        {
            // Act
            var result = await _controller.GetFilterOptions();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }
    }
}

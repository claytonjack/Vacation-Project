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
    public class VacationsControllerTests
    {
        private readonly VacationsController _controller;
        private readonly VacationDbContext _context;

        public VacationsControllerTests()
        {
            // Create in-memory database options
            var options = new DbContextOptionsBuilder<VacationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestVacationsDb")
                .Options;

            _context = new VacationDbContext(options);
            _context.Database.EnsureDeleted(); // Start fresh each test
            
            // Set up test data
            _context.Destinations.AddRange(
                new Destination { DestinationID = 1, City = "Paris", Country = "France" },
                new Destination { DestinationID = 2, City = "London", Country = "England" }
            );

            _context.Accommodations.Add(
                new Accommodation { 
                    AccommodationID = 1, 
                    HotelName = "Test Hotel", 
                    Address = "123 Test St", 
                    RoomType = "Standard" 
                }
            );

            _context.Vacations.AddRange(
                new Vacation {
                    VacationID = 1,
                    Name = "Paris Adventure",
                    Description = "Romantic getaway in Paris",
                    PricePerNight = 199.99m,
                    DestinationID = 1,
                    AccommodationID = 1,
                    AvailableRooms = 10
                },
                new Vacation {
                    VacationID = 2,
                    Name = "London City Break",
                    Description = "Explore historic London",
                    PricePerNight = 149.99m,
                    DestinationID = 2,
                    AccommodationID = 1,
                    AvailableRooms = 5
                }
            );

            // Add a booking for VacationID 1
            _context.Users.Add(
                new User { 
                    Id = "user1", 
                    UserName = "test@example.com", 
                    Email = "test@example.com",
                    FirstName = "Test",
                    LastName = "User",
                    Address = "456 User St"
                }
            );

            _context.Bookings.Add(
                new Booking {
                    BookingID = 1,
                    UserID = "user1",
                    VacationID = 1,
                    NumberOfNights = 3,
                    NumberOfGuests = 2,
                    CheckInDate = System.DateTime.Now.AddDays(10),
                    BookingDate = System.DateTime.Now,
                    TotalPrice = 599.97m,
                    SpecialRequests = "None"
                }
            );

            _context.SaveChanges();
            _controller = new VacationsController(_context);
        }

        // Happy path tests
        [Fact]
        public async Task GetVacations_ReturnsAllVacations()
        {
            // Act
            var result = await _controller.GetVacations();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Vacation>>>(result);
            var vacations = Assert.IsAssignableFrom<IEnumerable<Vacation>>(actionResult.Value);
            Assert.Equal(2, vacations.Count());
        }

        [Fact]
        public async Task GetVacation_WithValidId_ReturnsVacation()
        {
            // Act
            var result = await _controller.GetVacation(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Vacation>>(result);
            var vacation = Assert.IsType<Vacation>(actionResult.Value);
            Assert.Equal("Paris Adventure", vacation.Name);
        }

        [Fact]
        public async Task GetVacationsByDestination_ReturnsVacationsForDestination()
        {
            // Act
            var result = await _controller.GetVacationsByDestination(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Vacation>>>(result);
            var vacations = Assert.IsAssignableFrom<IEnumerable<Vacation>>(actionResult.Value);
            Assert.Single(vacations);
            Assert.Equal("Paris Adventure", vacations.First().Name);
        }

        [Fact]
        public async Task PostVacation_WithValidModel_AddsVacation()
        {
            // Arrange
            var newVacation = new Vacation
            {
                Name = "New Vacation",
                Description = "Test description",
                PricePerNight = 129.99m,
                DestinationID = 1,
                AccommodationID = 1,
                AvailableRooms = 3
            };

            // Act
            var result = await _controller.PostVacation(newVacation);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Vacation>>(result);
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.NotNull(createdResult.Value);
        }

        // Error path tests
        [Fact]
        public async Task GetVacation_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetVacation(999);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Vacation>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task PostVacation_WithInvalidDestination_ReturnsBadRequest()
        {
            // Arrange
            var invalidVacation = new Vacation
            {
                Name = "Invalid Vacation",
                Description = "Test description",
                PricePerNight = 129.99m,
                DestinationID = 999, // Invalid ID
                AccommodationID = 1,
                AvailableRooms = 3
            };

            // Act
            var result = await _controller.PostVacation(invalidVacation);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Vacation>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task DeleteVacation_WithBookings_ReturnsBadRequest()
        {
            // Act - try to delete a vacation with bookings
            var result = await _controller.DeleteVacation(1);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationBooking.Controllers;
using VacationBooking.Data;
using VacationBooking.Models;
using Xunit;

namespace Vacation_API.Tests.Controllers
{
    public class BookingsControllerTests
    {
        private readonly BookingsController _controller;
        private readonly VacationDbContext _context;

        public BookingsControllerTests()
        {
            // Simple in-memory database setup
            var options = new DbContextOptionsBuilder<VacationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestBookingsDb")
                .Options;

            _context = new VacationDbContext(options);
            _context.Database.EnsureDeleted(); // Start fresh each test
            
            // Setup minimal required data
            _context.Users.Add(new User { 
                Id = "user1", 
                UserName = "test@example.com",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                Address = "123 Test St"
            });
            
            _context.Destinations.Add(new Destination {
                DestinationID = 1,
                City = "Test City",
                Country = "Test Country"
            });
            
            _context.Accommodations.Add(new Accommodation {
                AccommodationID = 1,
                HotelName = "Test Hotel",
                Address = "123 Hotel St",
                RoomType = "Standard"
            });
            
            _context.Vacations.Add(new Vacation {
                VacationID = 1,
                Name = "Test Vacation",
                Description = "Test Description",
                PricePerNight = 100m,
                DestinationID = 1,
                AccommodationID = 1
            });
            
            _context.Bookings.Add(new Booking {
                BookingID = 1,
                UserID = "user1",
                VacationID = 1,
                CheckInDate = DateTime.Now.AddDays(5),
                NumberOfNights = 3,
                NumberOfGuests = 2,
                SpecialRequests = "None",
                TotalPrice = 300m
            });
            
            _context.SaveChanges();

            _controller = new BookingsController(_context);
        }

        // Happy path tests
        [Fact]
        public async void GetBookings_ReturnsBookings()
        {
            // Act
            var result = await _controller.GetBookings();

            // Assert
            Assert.NotNull(result.Value);
            Assert.NotEmpty(result.Value);
        }

        [Fact]
        public async void GetBooking_WithValidId_ReturnsBooking()
        {
            // Act
            var result = await _controller.GetBooking(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Booking>>(result);
            var item = Assert.IsType<Booking>(actionResult.Value);
            Assert.Equal(1, item.BookingID);
        }

        // Error path tests
        [Fact]
        public async void GetBooking_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetBooking(999);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Booking>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async void PostBooking_WithPastDate_ReturnsBadRequest()
        {
            // Arrange - create a booking with a past check-in date
            var invalidBooking = new Booking
            {
                UserID = "user1",
                VacationID = 1,
                CheckInDate = DateTime.Now.AddDays(-5), // Past date!
                NumberOfNights = 2,
                NumberOfGuests = 1,
                SpecialRequests = "None"
            };

            // Act
            var result = await _controller.PostBooking(invalidBooking);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Booking>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetBookingsByUser_ReturnsUserBookings()
        {
            // Act
            var result = await _controller.GetBookingsByUser("user1");

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Booking>>>(result);
            var bookings = Assert.IsAssignableFrom<IEnumerable<Booking>>(actionResult.Value);
            Assert.Single(bookings);
            Assert.Equal(1, bookings.First().BookingID);
        }

        [Fact]
        public async Task PostBooking_WithValidModel_AddsBooking()
        {
            // Create a new context to avoid tracking conflicts
            var options = new DbContextOptionsBuilder<VacationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestBookingsDb")
                .Options;
            var context = new VacationDbContext(options);
            var controller = new BookingsController(context);

            // Arrange
            var newBooking = new Booking
            {
                UserID = "user1",
                VacationID = 1,
                CheckInDate = DateTime.Now.AddDays(20),
                NumberOfNights = 2,
                NumberOfGuests = 1,
                SpecialRequests = "No special requests" // Add the missing required field
            };

            // Act
            var result = await controller.PostBooking(newBooking);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Booking>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnValue = Assert.IsType<Booking>(createdAtActionResult.Value);
            Assert.Equal("user1", returnValue.UserID);
            // Expected price depends on your vacation price
            Assert.Equal(200.00m, returnValue.TotalPrice); // 2 nights at $100 per night
        }

        [Fact]
        public async Task PutBooking_WithValidIdAndModel_ReturnsNoContent()
        {
            // Create a new context to avoid tracking conflicts
            var options = new DbContextOptionsBuilder<VacationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestBookingsDb")
                .Options;
            var context = new VacationDbContext(options);
            var controller = new BookingsController(context);

            // Arrange
            var booking = new Booking
            {
                BookingID = 1,
                UserID = "user1",
                VacationID = 1,
                CheckInDate = DateTime.Now.AddDays(15),
                NumberOfNights = 4,
                NumberOfGuests = 3,
                SpecialRequests = "Updated request" // Add the missing required field
            };

            // Act
            var result = await controller.PutBooking(1, booking);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteBooking_WithValidId_ReturnsNoContent()
        {
            // Act
            var result = await _controller.DeleteBooking(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteBooking_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.DeleteBooking(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}

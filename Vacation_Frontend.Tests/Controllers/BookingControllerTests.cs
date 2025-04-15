using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using VacationBooking.Controllers;
using VacationBooking.Models;
using VacationBooking.Services;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Vacation_Frontend.Tests.Controllers
{
    public class BookingControllerTests
    {
        private readonly Mock<IVacationApiService> _mockApiService;
        private readonly BookingController _controller;
        private readonly User _testUser;

        public BookingControllerTests()
        {
            _testUser = new User
            {
                Id = "user-1",
                UserName = "user@example.com",
                Email = "user@example.com"
            };

            // Mock the API service
            _mockApiService = new Mock<IVacationApiService>();
            
            // Set up the mock to return the test user
            _mockApiService
                .Setup(s => s.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(_testUser);
            
            // Create controller with mock API service
            _controller = new BookingController(_mockApiService.Object);
            
            // Set up controller context with a valid ClaimsPrincipal
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "user@example.com"),
                new Claim(ClaimTypes.NameIdentifier, "user-1")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        //booking with id test
        [Fact]
        public async Task NewBooking_WithValidId_ReturnsViewWithBooking()
        {
            var vacationId = 1;
            var vacation = new Vacation
            {
                VacationID = vacationId,
                Name = "London City Break",
                PricePerNight = 289.99m
            };

            // Setup GetVacationByIdAsync
            _mockApiService
                .Setup(m => m.GetVacationByIdAsync(vacationId))
                .ReturnsAsync(vacation);

            var result = await _controller.NewBooking(vacationId) as ViewResult;
            var model = result?.Model as Booking;

            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Equal(vacationId, model.VacationID);
            Assert.Equal(_testUser.Id, model.UserID);
            Assert.NotNull(model.Vacation);
            Assert.Equal("London City Break", model.Vacation.Name);
        }

        //confirmation of booking test
        [Fact]
        public async Task NewBookingPost_WithValidData_RedirectsToConfirmation()
        {
            var vacationId = 1;
            var vacation = new Vacation
            {
                VacationID = vacationId,
                Name = "London City Break",
                PricePerNight = 289.99m
            };
            
            var booking = new Booking
            {
                BookingID = 1,
                VacationID = vacationId,
                UserID = _testUser.Id,
                CheckInDate = DateTime.Now.AddDays(1),
                NumberOfNights = 3,
                NumberOfGuests = 2,
                TotalPrice = 3 * 289.99m
            };

            // Setup GetVacationByIdAsync
            _mockApiService
                .Setup(m => m.GetVacationByIdAsync(vacationId))
                .ReturnsAsync(vacation);
                
            // Setup CreateBookingAsync
            _mockApiService
                .Setup(m => m.CreateBookingAsync(It.IsAny<Booking>()))
                .ReturnsAsync(booking);

            var result = await _controller.NewBooking(
                vacationId,
                DateTime.Now.AddDays(1),
                3,
                2,
                "Special request notes") as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Confirmation", result.ActionName);
            Assert.Equal(booking.BookingID, result.RouteValues["id"]);
        }

        //confirmation of id test
        [Fact]
        public async Task Confirmation_WithValidId_ReturnsViewWithBooking()
        {
            var bookingId = 1;
            var booking = new Booking
            {
                BookingID = bookingId,
                VacationID = 1,
                UserID = _testUser.Id,
                CheckInDate = DateTime.Now.AddDays(1),
                NumberOfNights = 3,
                TotalPrice = 3 * 289.99m
            };

            // Setup GetBookingByIdAsync
            _mockApiService
                .Setup(m => m.GetBookingByIdAsync(bookingId))
                .ReturnsAsync(booking);

            var result = await _controller.Confirmation(bookingId) as ViewResult;
            var model = result?.Model as Booking;

            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Equal(bookingId, model.BookingID);
            Assert.Equal(_testUser.Id, model.UserID);
        }
    }
}

using Microsoft.AspNetCore.Identity;
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

namespace Vacation_Frontend.Tests.Controllers
{
    public class AdminControllerTests
    {
        private readonly Mock<IVacationApiService> _mockApiService;
        private readonly AdminController _controller;
        private readonly User _adminUser;

        public AdminControllerTests()
        {
            _adminUser = new User
            {
                Id = "admin-1",
                UserName = "admin@example.com",
                Email = "admin@example.com",
                IsAdmin = true
            };

            // Mock the API service instead of using a real instance
            _mockApiService = new Mock<IVacationApiService>();
            
            // Set up the mock to return the admin user when IsUserAdminAsync is called
            _mockApiService
                .Setup(s => s.IsUserAdminAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(true);
                
            _mockApiService
                .Setup(s => s.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(_adminUser);
            
            // Create controller with mock API service
            _controller = new AdminController(_mockApiService.Object);
            
            // Set up controller context with a valid ClaimsPrincipal
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "admin@example.com"),
                new Claim(ClaimTypes.NameIdentifier, "admin-1")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        //admin user test
        [Fact]
        public async Task Dashboard_AdminUser_ReturnsViewWithVacations()
        {
            // Set up mock to return test vacations
            var vacations = new List<Vacation>
            {
                new Vacation { VacationID = 1, Name = "London City Break", PricePerNight = 289.99m },
                new Vacation { VacationID = 2, Name = "Paris Romantic Escape", PricePerNight = 249.99m }
            };
            
            _mockApiService
                .Setup(s => s.GetAllVacationsAsync())
                .ReturnsAsync(vacations);

            var result = await _controller.Dashboard() as ViewResult;
            var model = result?.Model as List<Vacation>;

            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Equal(2, model.Count);
            Assert.Equal("London City Break", model[0].Name);
            Assert.Equal("Paris Romantic Escape", model[1].Name);
        }

        //admin create test
        [Fact]
        public async Task Create_AdminUser_ReturnsViewWithData()
        {
            var destinations = new List<Destination>
            {
                new Destination { DestinationID = 1, City = "London", Country = "England" }
            };

            var accommodations = new List<Accommodation>
            {
                new Accommodation { AccommodationID = 1, HotelName = "The Grand Hotel" }
            };

            _mockApiService
                .Setup(s => s.GetAllDestinationsAsync())
                .ReturnsAsync(destinations);

            _mockApiService
                .Setup(s => s.GetAllAccommodationsAsync())
                .ReturnsAsync(accommodations);

            var result = await _controller.Create() as ViewResult;
            var model = result?.Model as Vacation;
            var resultDestinations = result?.ViewData["Destinations"] as List<Destination>;
            var resultAccommodations = result?.ViewData["Accommodations"] as List<Accommodation>;

            Assert.NotNull(result);
            Assert.NotNull(model);
            
            Assert.NotNull(resultDestinations);
            Assert.Equal(destinations.Count, resultDestinations.Count);
            Assert.Equal(destinations[0].DestinationID, resultDestinations[0].DestinationID);
            Assert.Equal(destinations[0].City, resultDestinations[0].City);
            Assert.Equal(destinations[0].Country, resultDestinations[0].Country);
            
            Assert.NotNull(resultAccommodations);
            Assert.Equal(accommodations.Count, resultAccommodations.Count);
            Assert.Equal(accommodations[0].AccommodationID, resultAccommodations[0].AccommodationID);
            Assert.Equal(accommodations[0].HotelName, resultAccommodations[0].HotelName);
        }
    }
}

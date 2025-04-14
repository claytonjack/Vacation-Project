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

namespace Vacation_Frontend.Tests.Controllers
{
    public class AdminControllerTests
    {
        private readonly VacationApiService _apiService;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly AdminController _controller;
        private readonly User _adminUser;
        private readonly Mock<HttpMessageHandler> _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public AdminControllerTests()
        {
            _adminUser = new User
            {
                Id = "admin-1",
                UserName = "admin@example.com",
                Email = "admin@example.com",
                IsAdmin = true
            };

            var userStoreMock = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);
            
            _mockUserManager
                .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(_adminUser);

            _mockHttpHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://test-api.com")
            };

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["ApiSettings:BaseUrl"]).Returns("http://test-api.com");

            _apiService = new VacationApiService(_httpClient, _mockConfiguration.Object);
            
            _controller = new AdminController(_apiService, _mockUserManager.Object);
        }

        //admin user test
        [Fact]
        public async Task Dashboard_AdminUser_ReturnsViewWithVacations()
        {
            var vacations = new List<Vacation>
            {
                new Vacation { VacationID = 1, Name = "London City Break", PricePerNight = 289.99m },
                new Vacation { VacationID = 2, Name = "Paris Romantic Escape", PricePerNight = 249.99m }
            };

            var jsonResponse = JsonSerializer.Serialize(vacations);
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => 
                        req.Method == HttpMethod.Get && 
                        req.RequestUri.ToString().Contains("vacations")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

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

            var destJsonResponse = JsonSerializer.Serialize(destinations);
            var destResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(destJsonResponse, Encoding.UTF8, "application/json")
            };

            var accomJsonResponse = JsonSerializer.Serialize(accommodations);
            var accomResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(accomJsonResponse, Encoding.UTF8, "application/json")
            };

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => 
                        req.Method == HttpMethod.Get && 
                        req.RequestUri.ToString().Contains("destinations")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(destResponse);

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => 
                        req.Method == HttpMethod.Get && 
                        req.RequestUri.ToString().Contains("accommodations")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(accomResponse);

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

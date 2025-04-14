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
    public class BookingControllerTests
    {
        private readonly VacationApiService _apiService;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly BookingController _controller;
        private readonly User _testUser;
        private readonly Mock<HttpMessageHandler> _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public BookingControllerTests()
        {
            _testUser = new User
            {
                Id = "user-1",
                UserName = "test@example.com",
                Email = "test@example.com"
            };

            var userStoreMock = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);
            
            _mockUserManager
                .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(_testUser);

            _mockHttpHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://test-api.com")
            };

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["ApiSettings:BaseUrl"]).Returns("http://test-api.com");

            _apiService = new VacationApiService(_httpClient, _mockConfiguration.Object);
            
            _controller = new BookingController(_apiService, _mockUserManager.Object);
        }

        //bookinng with id test
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

            var jsonResponse = JsonSerializer.Serialize(vacation);
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
                        req.RequestUri.ToString().Contains($"/vacations/{vacationId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

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

            var vacationJsonResponse = JsonSerializer.Serialize(vacation);
            var vacationResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(vacationJsonResponse, Encoding.UTF8, "application/json")
            };

            var bookingJsonResponse = JsonSerializer.Serialize(booking);
            var bookingResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Created,
                Content = new StringContent(bookingJsonResponse, Encoding.UTF8, "application/json")
            };

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => 
                        req.Method == HttpMethod.Get && 
                        req.RequestUri.ToString().Contains($"/vacations/{vacationId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(vacationResponse);

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => 
                        req.Method == HttpMethod.Post && 
                        req.RequestUri.ToString().Contains("bookings")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(bookingResponse);

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

        //connfirmation of id test
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

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _testUser.Id)
            }));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = user }
            };

            var jsonResponse = JsonSerializer.Serialize(booking);
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
                        req.RequestUri.ToString().Contains($"bookings/{bookingId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var result = await _controller.Confirmation(bookingId) as ViewResult;
            var model = result?.Model as Booking;

            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Equal(bookingId, model.BookingID);
            Assert.Equal(_testUser.Id, model.UserID);
        }
    }
}

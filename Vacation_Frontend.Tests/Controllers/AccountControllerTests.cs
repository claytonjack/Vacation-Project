using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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
    public class AccountControllerTests
    {
        private readonly VacationApiService _apiService;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<SignInManager<User>> _mockSignInManager;
        private readonly AccountController _controller;
        private readonly User _testUser;
        private readonly Mock<HttpMessageHandler> _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public AccountControllerTests()
        {
            _testUser = new User
            {
                Id = "user-1",
                UserName = "test@example.com",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User"
            };

            var userStoreMock = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);
            
            _mockUserManager
                .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(_testUser);

            _mockUserManager
                .Setup(um => um.FindByEmailAsync(It.Is<string>(s => s == _testUser.Email)))
                .ReturnsAsync(_testUser);

            _mockUserManager
                .Setup(um => um.FindByEmailAsync(It.Is<string>(s => s != _testUser.Email)))
                .ReturnsAsync((User)null);

            _mockUserManager
                .Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object, contextAccessorMock.Object, userPrincipalFactoryMock.Object, 
                null, null, null, null);

            _mockSignInManager
                .Setup(sm => sm.PasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _mockHttpHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://test-api.com")
            };

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["ApiSettings:BaseUrl"]).Returns("http://test-api.com");

            _apiService = new VacationApiService(_httpClient, _mockConfiguration.Object);
            
            _controller = new AccountController(_apiService, _mockUserManager.Object, _mockSignInManager.Object);
        }

        [Fact]
        public void Register_ReturnsView()
        {
            var result = _controller.Register();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Register_WithValidModel_RedirectsToHome()
        {
            var newUser = new User
            {
                Email = "new@example.com",
                FirstName = "New",
                LastName = "User",
                Password = "Password123!"
            };

            var result = await _controller.Register(newUser) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
            _mockUserManager.Verify(um => um.CreateAsync(It.IsAny<User>(), "Password123!"), Times.Once);
            _mockSignInManager.Verify(sm => sm.SignInAsync(It.IsAny<User>(), false, null), Times.Once);
        }

        [Fact]
        public void Login_ReturnsViewWithModel()
        {
            var httpContext = new DefaultHttpContext();
            var routeData = new RouteData();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext,
                RouteData = routeData
            };
            
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(u => u.Content(It.IsAny<string>())).Returns("~/");
            _controller.Url = urlHelper.Object;
            
            var result = _controller.Login(returnUrl: null) as ViewResult;
            var model = result?.Model as Login;

            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Equal("~/", model.ReturnUrl);
        }

        [Fact]
        public async Task Login_WithValidCredentials_RedirectsToHome()
        {
            var login = new Login
            {
                Email = "test@example.com",
                Password = "Password123!",
                Remember = false
            };

            var result = await _controller.Login(login) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
            _mockSignInManager.Verify(sm => sm.SignOutAsync(), Times.Once);
            _mockSignInManager.Verify(sm => sm.PasswordSignInAsync(
                It.IsAny<User>(), login.Password, login.Remember, false), Times.Once);
        }

        [Fact]
        public async Task Profile_AuthenticatedUser_ReturnsViewWithUserAndBookings()
        {
            var bookings = new List<Booking>
            {
                new Booking { BookingID = 1, VacationID = 1, UserID = _testUser.Id }
            };

            var jsonResponse = JsonSerializer.Serialize(bookings);
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
                        req.RequestUri.ToString().Contains($"bookings/user/{_testUser.Id}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var result = await _controller.Profile() as ViewResult;
            var model = result?.Model as User;
            var resultBookings = result?.ViewData["Bookings"] as List<Booking>;

            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Equal(_testUser.Id, model.Id);
            Assert.Equal(_testUser.Email, model.Email);
            
            Assert.NotNull(resultBookings);
            Assert.Equal(bookings.Count, resultBookings.Count);
            Assert.Equal(bookings[0].BookingID, resultBookings[0].BookingID);
            Assert.Equal(bookings[0].UserID, resultBookings[0].UserID);
        }

        [Fact]
        public async Task Logout_RedirectsToHome()
        {
            var result = await _controller.Logout() as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
            _mockSignInManager.Verify(sm => sm.SignOutAsync(), Times.Once);
        }
    }
}

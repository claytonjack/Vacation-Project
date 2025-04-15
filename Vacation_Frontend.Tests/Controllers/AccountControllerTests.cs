using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
        private readonly AccountController _controller;
        private readonly Mock<HttpMessageHandler> _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly string _baseUrl = "http://test-api.com/api";
        private readonly Mock<IAuthenticationService> _mockAuthService;
        private readonly Mock<ITempDataDictionaryFactory> _mockTempDataDictionaryFactory;

        public AccountControllerTests()
        {
            // Setup configuration
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["ApiSettings:BaseUrl"]).Returns(_baseUrl);

            // Setup HTTP handler
            _mockHttpHandler = new Mock<HttpMessageHandler>();
            
            // Configure mock HTTP responses
            SetupMockHttpResponses();
            
            // Create HTTP client with mock handler
            _httpClient = new HttpClient(_mockHttpHandler.Object)
            {
                BaseAddress = new Uri(_baseUrl)
            };

            // Create API service
            _apiService = new VacationApiService(_httpClient, _mockConfiguration.Object);
            
            // Setup auth service mock
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockAuthService
                .Setup(x => x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);
                
            _mockAuthService
                .Setup(x => x.SignOutAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);
                
            // Setup TempData mock
            _mockTempDataDictionaryFactory = new Mock<ITempDataDictionaryFactory>();
            var tempDataMock = new Mock<ITempDataDictionary>();
            _mockTempDataDictionaryFactory
                .Setup(factory => factory.GetTempData(It.IsAny<HttpContext>()))
                .Returns(tempDataMock.Object);
            
            // Create controller
            _controller = new AccountController(_apiService);
            
            // Setup HttpContext with auth service and TempData
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
                .Returns(_mockAuthService.Object);
                
            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(ITempDataDictionaryFactory)))
                .Returns(_mockTempDataDictionaryFactory.Object);
                
            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object
            };
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            
            // Set up controller with URL helper and TempData for views
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(u => u.Content(It.IsAny<string>())).Returns("~/");
            _controller.Url = urlHelperMock.Object;
            _controller.TempData = tempDataMock.Object;
        }

        private void SetupMockHttpResponses()
        {
            // Create successful responses for all API calls
            var successResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                    JsonSerializer.Serialize(new AuthResponse 
                    { 
                        Success = true,
                        UserId = "user-1",
                        Email = "test@example.com",
                        FirstName = "Test",
                        LastName = "User",
                        Roles = new List<string> { "User" }
                    }),
                    Encoding.UTF8, 
                    "application/json")
            };

            // Default handler setup for any request
            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(successResponse);
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
            var model = new User
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "Test",
                LastName = "User"
            };

            var result = await _controller.Register(model);
            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }

        [Fact]
        public void Login_ReturnsView()
        {
            var result = _controller.Login();
            
            Assert.IsType<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsType<Login>(viewResult.Model);
        }

        [Fact]
        public async Task Login_WithValidCredentials_RedirectsToHome()
        {
            var login = new Login
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            var result = await _controller.Login(login);
            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Logout_RedirectsToHome()
        {
            var result = await _controller.Logout();
            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;
using VacationBooking.Controllers;
using VacationBooking.Models;
using VacationBooking.Services;
using Xunit;

namespace Vacation_Frontend.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly VacationApiService _apiService;
        private readonly HomeController _controller;
        private readonly Mock<HttpMessageHandler> _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public HomeControllerTests()
        {
            _mockHttpHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://test-api.com")
            };

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["ApiSettings:BaseUrl"]).Returns("http://test-api.com");

            _apiService = new VacationApiService(_httpClient, _mockConfiguration.Object);
            
            _controller = new HomeController(_apiService);
        }

        [Fact]
        public async Task Index_ReturnsViewWithVacations()
        {
            var vacations = new List<Vacation>
            {
                new Vacation { VacationID = 1, Name = "London City Break", PricePerNight = 289.99m },
                new Vacation { VacationID = 2, Name = "Paris Romantic Escape", PricePerNight = 249.99m },
                new Vacation { VacationID = 3, Name = "Barcelona Beach Adventure", PricePerNight = 219.99m },
                new Vacation { VacationID = 4, Name = "Prague Fairytale Getaway", PricePerNight = 199.99m }
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

            var result = await _controller.Index() as ViewResult;
            var model = result?.Model as List<Vacation>;

            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.True(model.Count <= 3);
        }

        [Fact]
        public void Privacy_ReturnsView()
        {
            var result = _controller.Privacy() as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void Error_ReturnsViewWithErrorViewModel()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "test-trace-id";
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var result = _controller.Error() as ViewResult;
            var model = result?.Model as ErrorViewModel;

            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Equal("test-trace-id", model.RequestId);
        }
    }
}

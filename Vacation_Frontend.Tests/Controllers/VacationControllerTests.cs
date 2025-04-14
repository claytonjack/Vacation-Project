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
    public class VacationControllerTests
    {
        private readonly VacationApiService _apiService;
        private readonly VacationController _controller;
        private readonly Mock<HttpMessageHandler> _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public VacationControllerTests()
        {
            _mockHttpHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://test-api.com")
            };

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["ApiSettings:BaseUrl"]).Returns("http://test-api.com");

            _apiService = new VacationApiService(_httpClient, _mockConfiguration.Object);
            
            _controller = new VacationController(_apiService);
        }

        //search of view test
        [Fact]
        public void Search_ReturnsViewWithSearchCriteria()
        {
            var result = _controller.Search() as ViewResult;
            var model = result?.Model as SearchCriteria;

            Assert.NotNull(result);
            Assert.NotNull(model);
        }

        //results of view test
        [Fact]
        public async Task Results_ReturnsViewWithSearchResults()
        {
            var criteria = new SearchCriteria 
            { 
                City = "London", 
                Country = "England", 
                MaxPricePerNight = 300 
            };
            
            var vacations = new List<Vacation>
            {
                new Vacation { VacationID = 1, Name = "London City Break", PricePerNight = 289.99m }
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
                        req.Method == HttpMethod.Post && 
                        req.RequestUri.ToString().Contains("vacationsearch")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var result = await _controller.Results(criteria) as ViewResult;
            var model = result?.Model as List<Vacation>;

            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Single(model);
            Assert.Equal("London City Break", model[0].Name);
            Assert.Equal(criteria, result.ViewData["SearchCriteria"]);
        }

        //vaction detail with id test
        [Fact]
        public async Task Details_WithValidId_ReturnsViewWithVacation()
        {
            var id = 1;
            var vacation = new Vacation 
            { 
                VacationID = id, 
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
                        req.RequestUri.ToString().Contains($"vacations/{id}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var result = await _controller.Details(id) as ViewResult;
            var model = result?.Model as Vacation;

            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Equal(id, model.VacationID);
            Assert.Equal("London City Break", model.Name);
        }

        //vaction with invalid id test
        [Fact]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            var id = 999;
            
            var jsonResponse = "null";
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
                        req.RequestUri.ToString().Contains($"vacations/{id}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var result = await _controller.Details(id);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}

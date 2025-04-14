using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;
using VacationBooking.Models;
using VacationBooking.Services;

namespace Vacation_Frontend.Tests.Services
{
    public class VacationApiServiceTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly VacationApiService _apiService;
        private readonly string _baseUrl = "http://localhost:5000/api";

        public VacationApiServiceTests()
        {
            // Setup mock HTTP handler
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            // Create HttpClient with the mock handler
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(_baseUrl)
            };

            // Setup mock configuration
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["ApiSettings:BaseUrl"]).Returns(_baseUrl);

            // Create service with mocked dependencies
            _apiService = new VacationApiService(_httpClient, _mockConfiguration.Object);
        }

        [Fact]
        public async Task GetAllVacationsAsync_ReturnsVacations()
        {
            // Arrange
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

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            // Act
            var result = await _apiService.GetAllVacationsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("London City Break", result[0].Name);
            Assert.Equal("Paris Romantic Escape", result[1].Name);
        }

        [Fact]
        public async Task GetVacationByIdAsync_ReturnsVacation()
        {
            // Arrange
            var vacation = new Vacation 
            { 
                VacationID = 1, 
                Name = "London City Break", 
                PricePerNight = 289.99m,
                Description = "Experience the historic charm and modern excitement of London..."
            };

            var jsonResponse = JsonSerializer.Serialize(vacation);
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            // Act
            var result = await _apiService.GetVacationByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.VacationID);
            Assert.Equal("London City Break", result.Name);
            Assert.Equal(289.99m, result.PricePerNight);
        }

        [Fact]
        public async Task CreateVacationAsync_ReturnsCreatedVacation()
        {
            // Arrange
            var newVacation = new Vacation
            {
                Name = "New Vacation",
                Description = "A brand new vacation package",
                PricePerNight = 199.99m,
                DestinationID = 1,
                AccommodationID = 1
            };

            var createdVacation = new Vacation
            {
                VacationID = 3,
                Name = "New Vacation",
                Description = "A brand new vacation package",
                PricePerNight = 199.99m,
                DestinationID = 1,
                AccommodationID = 1
            };

            var jsonResponse = JsonSerializer.Serialize(createdVacation);
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Created,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            // Act
            var result = await _apiService.CreateVacationAsync(newVacation);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.VacationID);
            Assert.Equal("New Vacation", result.Name);
            Assert.Equal(199.99m, result.PricePerNight);
        }

        [Fact]
        public async Task UpdateVacationAsync_CallsApiWithCorrectData()
        {
            // Arrange
            var vacationId = 1;
            var vacation = new Vacation
            {
                VacationID = vacationId,
                Name = "Updated Vacation",
                PricePerNight = 299.99m
            };

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            // Act & Assert (no exception means success)
            await _apiService.UpdateVacationAsync(vacationId, vacation);

            _mockHttpMessageHandler
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(req => 
                        req.Method == HttpMethod.Put && 
                        req.RequestUri.ToString().EndsWith($"/vacations/{vacationId}")),
                    ItExpr.IsAny<CancellationToken>()
                );
        }

        [Fact]
        public async Task DeleteVacationAsync_CallsApiWithCorrectId()
        {
            // Arrange
            var vacationId = 1;
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            // Act & Assert (no exception means success)
            await _apiService.DeleteVacationAsync(vacationId);

            _mockHttpMessageHandler
                .Protected()
                .Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(req => 
                        req.Method == HttpMethod.Delete && 
                        req.RequestUri.ToString().EndsWith($"/vacations/{vacationId}")),
                    ItExpr.IsAny<CancellationToken>()
                );
        }

        [Fact]
        public async Task SearchVacationsAsync_WithCriteria_ReturnsFilteredVacations()
        {
            // Arrange
            var criteria = new SearchCriteria
            {
                City = "London",
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

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            // Act
            var result = await _apiService.SearchVacationsAsync(criteria);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("London City Break", result[0].Name);
        }
    }
}

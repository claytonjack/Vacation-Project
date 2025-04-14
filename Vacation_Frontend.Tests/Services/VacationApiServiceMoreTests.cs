using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VacationBooking.Models;
using VacationBooking.Services;
using Xunit;

namespace Vacation_Frontend.Tests.Services
{
    public class VacationApiServiceMoreTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly VacationApiService _apiService;
        private readonly string _baseUrl = "http://localhost:5000/api";

        public VacationApiServiceMoreTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(_baseUrl)
            };
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["ApiSettings:BaseUrl"]).Returns(_baseUrl);
            _apiService = new VacationApiService(_httpClient, _mockConfiguration.Object);
        }

        [Fact]
        public async Task GetUserBookingsAsync_ReturnsUserBookings()
        {
            // Arrange
            var userId = "user123";
            var bookings = new List<Booking>
            {
                new Booking { BookingID = 1, UserID = userId, VacationID = 1 },
                new Booking { BookingID = 2, UserID = userId, VacationID = 2 }
            };

            var jsonResponse = JsonSerializer.Serialize(bookings);
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => 
                        req.Method == HttpMethod.Get && 
                        req.RequestUri.ToString().Contains($"bookings/user/{userId}")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            // Act
            var result = await _apiService.GetUserBookingsAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].BookingID);
            Assert.Equal(2, result[1].BookingID);
            Assert.Equal(userId, result[0].UserID);
        }

        [Fact]
        public async Task GetAllDestinationsAsync_ReturnsDestinations()
        {
            // Arrange
            var destinations = new List<Destination>
            {
                new Destination { DestinationID = 1, City = "London", Country = "England" },
                new Destination { DestinationID = 2, City = "Paris", Country = "France" }
            };

            var jsonResponse = JsonSerializer.Serialize(destinations);
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => 
                        req.Method == HttpMethod.Get && 
                        req.RequestUri.ToString().EndsWith("destinations")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            // Act
            var result = await _apiService.GetAllDestinationsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("London", result[0].City);
            Assert.Equal("Paris", result[1].City);
        }

        [Fact]
        public async Task GetVacationsByDestinationAsync_ReturnsFilteredVacations()
        {
            // Arrange
            var destinationId = 1;
            var vacations = new List<Vacation>
            {
                new Vacation { VacationID = 1, Name = "London City Break", DestinationID = destinationId },
                new Vacation { VacationID = 2, Name = "London Cultural Tour", DestinationID = destinationId }
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
                    ItExpr.Is<HttpRequestMessage>(req => 
                        req.Method == HttpMethod.Get && 
                        req.RequestUri.ToString().Contains($"vacations/destination/{destinationId}")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            // Act
            var result = await _apiService.GetVacationsByDestinationAsync(destinationId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("London City Break", result[0].Name);
            Assert.Equal("London Cultural Tour", result[1].Name);
            Assert.Equal(destinationId, result[0].DestinationID);
        }
    }
} 
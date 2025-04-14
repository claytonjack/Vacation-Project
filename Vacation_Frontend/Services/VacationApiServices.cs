using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using VacationBooking.Data;
using VacationBooking.Models;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using VacationBooking.Services;

namespace VacationBooking.Services
{
    public class VacationApiService : IVacationApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public VacationApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"];
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            };
        }

        private string GetApiUrl(string endpoint)
        {
            if (!endpoint.StartsWith("/"))
            {
                endpoint = "/" + endpoint;
            }
            
            return endpoint;
        }

        #region Accommodation Methods
        public async Task<List<Accommodation>> GetAllAccommodationsAsync()
        {
            var response = await _httpClient.GetAsync(GetApiUrl("accommodations"));
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<List<Accommodation>>(_jsonOptions);
        }

        public async Task<Accommodation> GetAccommodationByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{GetApiUrl("accommodations")}/{id}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<Accommodation>(_jsonOptions);
        }

        public async Task<Accommodation> CreateAccommodationAsync(Accommodation accommodation)
        {
            var response = await _httpClient.PostAsJsonAsync(GetApiUrl("accommodations"), accommodation);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<Accommodation>(_jsonOptions);
        }

        public async Task UpdateAccommodationAsync(int id, Accommodation accommodation)
        {
            var response = await _httpClient.PutAsJsonAsync($"{GetApiUrl("accommodations")}/{id}", accommodation);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAccommodationAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{GetApiUrl("accommodations")}/{id}");
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Destination Methods
        public async Task<List<Destination>> GetAllDestinationsAsync()
        {
            var response = await _httpClient.GetAsync(GetApiUrl("destinations"));
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<List<Destination>>(_jsonOptions);
        }

        public async Task<Destination> GetDestinationByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{GetApiUrl("destinations")}/{id}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<Destination>(_jsonOptions);
        }

        public async Task<List<Destination>> SearchDestinationsAsync(string query)
        {
            var response = await _httpClient.GetAsync($"{GetApiUrl("destinations")}/search?query={Uri.EscapeDataString(query)}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<List<Destination>>(_jsonOptions);
        }

        public async Task<Destination> CreateDestinationAsync(Destination destination)
        {
            var response = await _httpClient.PostAsJsonAsync(GetApiUrl("destinations"), destination);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<Destination>(_jsonOptions);
        }

        public async Task UpdateDestinationAsync(int id, Destination destination)
        {
            var response = await _httpClient.PutAsJsonAsync($"{GetApiUrl("destinations")}/{id}", destination);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteDestinationAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{GetApiUrl("destinations")}/{id}");
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Vacation Methods
        public async Task<List<Vacation>> GetAllVacationsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/vacations");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<List<Vacation>>(_jsonOptions);
        }

        public async Task<Vacation> GetVacationByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/vacations/{id}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<Vacation>(_jsonOptions);
        }

        public async Task<List<Vacation>> SearchVacationsAsync(string query)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/vacations/search?query={Uri.EscapeDataString(query)}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<List<Vacation>>(_jsonOptions);
        }

        public async Task<List<Vacation>> SearchVacationsAsync(SearchCriteria criteria)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/vacationsearch", criteria);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<List<Vacation>>(_jsonOptions);
        }

        public async Task<List<Vacation>> GetVacationsByDestinationAsync(int destinationId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/vacations/destination/{destinationId}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<List<Vacation>>(_jsonOptions);
        }

        public async Task<Vacation> CreateVacationAsync(Vacation vacation)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/vacations", vacation);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<Vacation>(_jsonOptions);
        }

        public async Task UpdateVacationAsync(int id, Vacation vacation)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/vacations/{id}", vacation);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteVacationAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/vacations/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<object> GetFilterOptionsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/vacationsearch/getfilteroptions");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }
        #endregion

        #region Booking Methods
        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            var response = await _httpClient.GetAsync(GetApiUrl("bookings"));
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<List<Booking>>(_jsonOptions);
        }

        public async Task<Booking> GetBookingByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{GetApiUrl("bookings")}/{id}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<Booking>(_jsonOptions);
        }

        public async Task<List<Booking>> GetUserBookingsAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"{GetApiUrl("bookings")}/user/{userId}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<List<Booking>>(_jsonOptions);
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            var response = await _httpClient.PostAsJsonAsync(GetApiUrl("bookings"), booking);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<Booking>(_jsonOptions);
        }

        public async Task UpdateBookingAsync(int id, Booking booking)
        {
            var response = await _httpClient.PutAsJsonAsync($"{GetApiUrl("bookings")}/{id}", booking);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteBookingAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{GetApiUrl("bookings")}/{id}");
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region User Methods
        public async Task<List<User>> GetAllUsersAsync()
        {
            var response = await _httpClient.GetAsync(GetApiUrl("users"));
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<List<User>>(_jsonOptions);
        }

        public async Task<User> GetUserAsync(string id)
        {
            var response = await _httpClient.GetAsync($"{GetApiUrl("users")}/{id}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<User>(_jsonOptions);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            var response = await _httpClient.PostAsJsonAsync(GetApiUrl("users"), user);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<User>(_jsonOptions);
        }

        public async Task UpdateUserAsync(string id, User user)
        {
            var response = await _httpClient.PutAsJsonAsync($"{GetApiUrl("users")}/{id}", user);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteUserAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{GetApiUrl("users")}/{id}");
            response.EnsureSuccessStatusCode();
        }
        #endregion

        // Method to upload images through the API
        public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string entityType, int entityId)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(imageStream), "file", fileName);
            
            var response = await _httpClient.PostAsync($"{_baseUrl}/{entityType}/{entityId}/upload-image", content);
            response.EnsureSuccessStatusCode();
            
            var imageUrl = await response.Content.ReadAsStringAsync();
            return imageUrl;
        }
    }
}
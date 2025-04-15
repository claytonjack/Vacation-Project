using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using VacationBooking.Models;
using Microsoft.Extensions.Configuration;
using VacationBooking.Services;
using System.Security.Claims;

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
            
            // Make sure this is properly set from configuration
            _baseUrl = configuration["ApiSettings:BaseUrl"];
            
            // Add this line to Debug
            Console.WriteLine($"API Base URL: {_baseUrl}");
            
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
            
            return _baseUrl + endpoint;
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
            try 
            {
                // Make sure criteria object is not null
                if (criteria == null)
                    criteria = new SearchCriteria();
                    
                // Ensure empty strings rather than nulls for string properties
                criteria.City ??= string.Empty;
                criteria.Country ??= string.Empty;
                criteria.RoomType ??= string.Empty;
                
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/vacationsearch", criteria);
                
                // Handle specific response status
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Server rejected search criteria: {errorContent}");
                }
                
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadFromJsonAsync<List<Vacation>>(_jsonOptions);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error searching vacations: {ex.Message}");
                throw; // Rethrow to be caught by the controller
            }
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
            try {
                var response = await _httpClient.GetAsync($"{GetApiUrl("bookings")}/{id}");
                response.EnsureSuccessStatusCode();
                
                // For debugging - log the raw JSON
                var jsonContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Booking API Response: {jsonContent}");
                
                return await response.Content.ReadFromJsonAsync<Booking>(_jsonOptions);
            }
            catch (Exception ex) {
                Console.WriteLine($"Error getting booking {id}: {ex.Message}");
                
                // Try alternate approach - get booking without navigation properties
                // In case the API is failing due to circular references or navigation property issues
                var bookingResponse = await _httpClient.GetAsync($"{GetApiUrl("bookings")}/{id}?includeRelated=false");
                
                if (bookingResponse.IsSuccessStatusCode) {
                    var booking = await bookingResponse.Content.ReadFromJsonAsync<Booking>(_jsonOptions);
                    
                    // Manually load the vacation if needed
                    if (booking.VacationID > 0) {
                        try {
                            booking.Vacation = await GetVacationByIdAsync(booking.VacationID);
                        }
                        catch (Exception vacEx) {
                            Console.WriteLine($"Failed to load vacation for booking: {vacEx.Message}");
                        }
                    }
                    
                    return booking;
                }
                
                throw; // Re-throw if alternate approach also fails
            }
        }

        public async Task<List<Booking>> GetUserBookingsAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"{GetApiUrl("bookings")}/user/{userId}");
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<List<Booking>>(_jsonOptions);
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            try
            {
                // Get the complete user and vacation objects
                var user = await GetUserAsync(booking.UserID);
                var vacation = await GetVacationByIdAsync(booking.VacationID);
                
                // Create a booking object with complete User and Vacation properties
                var bookingData = new
                {
                    UserID = booking.UserID,
                    VacationID = booking.VacationID,
                    CheckInDate = booking.CheckInDate,
                    NumberOfNights = booking.NumberOfNights,
                    NumberOfGuests = booking.NumberOfGuests,
                    SpecialRequests = booking.SpecialRequests,
                    BookingDate = DateTime.Now,
                    // Complete User object
                    User = new {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Address = user.Address, 
                        Password = "TemporaryPassword123!", // Dummy password to pass validation
                        UserName = user.UserName,
                        Email = user.Email
                    },
                    // Complete Vacation object
                    Vacation = new {
                        VacationID = vacation.VacationID,
                        Name = vacation.Name,
                        Description = vacation.Description,
                        PricePerNight = vacation.PricePerNight,
                        DestinationID = vacation.DestinationID,
                        AccommodationID = vacation.AccommodationID,
                        AvailableRooms = vacation.AvailableRooms
                    }
                };
                
                var json = JsonSerializer.Serialize(bookingData);
                Console.WriteLine($"Sending booking data: {json}");
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(GetApiUrl("bookings"), content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Booking error response: {errorContent}");
                    throw new Exception($"Failed to create booking: {errorContent}");
                }
                
                return await response.Content.ReadFromJsonAsync<Booking>(_jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in CreateBookingAsync: {ex.Message}");
                throw;
            }
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
            
            // For debugging - log the raw JSON
            var jsonContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"User API Response: {jsonContent}");
            
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

        #region Auth Methods
        public async Task<AuthResponse> LoginAsync(string email, string password, bool remember)
        {
            var login = new LoginRequest
            {
                Email = email,
                Password = password,
                Remember = remember
            };
            
            _httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            var response = await _httpClient.PostAsJsonAsync(GetApiUrl("accounts/login"), login);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponse>(_jsonOptions);
            }
            
            return new AuthResponse
            {
                Success = false,
                Errors = new List<string> { "Failed to login" }
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest register)
        {
            _httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            var response = await _httpClient.PostAsJsonAsync(GetApiUrl("accounts/register"), register);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponse>(_jsonOptions);
            }
            
            return new AuthResponse
            {
                Success = false,
                Errors = new List<string> { "Failed to register" }
            };
        }

        public async Task<bool> LogoutAsync()
        {
            var response = await _httpClient.PostAsync(GetApiUrl("accounts/logout"), null);
            return response.IsSuccessStatusCode;
        }
        #endregion

        public async Task<User> GetCurrentUserAsync(ClaimsPrincipal userClaims)
        {
            var userId = userClaims.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            
            return await GetUserAsync(userId);
        }

        public async Task<bool> IsUserAdminAsync(ClaimsPrincipal userClaims)
        {
            // First check the claims directly - faster than an API call
            var isAdminClaim = userClaims.FindFirstValue("IsAdmin");
            if (!string.IsNullOrEmpty(isAdminClaim) && bool.TryParse(isAdminClaim, out bool isAdmin))
            {
                return isAdmin;
            }
            
            // If not found in claims, check via user object from API
            var user = await GetCurrentUserAsync(userClaims);
            return user != null && user.IsAdmin;
        }
    }
}
using VacationBooking.Models;
using System.Security.Claims;

namespace VacationBooking.Services
{
    public interface IVacationApiService
    {
        Task<List<Vacation>> GetAllVacationsAsync();
        Task<Vacation> GetVacationByIdAsync(int id);
        Task<List<Vacation>> SearchVacationsAsync(string query);
        Task<List<Vacation>> SearchVacationsAsync(SearchCriteria criteria);
        Task<List<Vacation>> GetVacationsByDestinationAsync(int destinationId);
        Task<Vacation> CreateVacationAsync(Vacation vacation);
        Task UpdateVacationAsync(int id, Vacation vacation);
        Task DeleteVacationAsync(int id);
        Task<object> GetFilterOptionsAsync();
        Task<List<Destination>> GetAllDestinationsAsync();
        Task<Destination> GetDestinationByIdAsync(int id);
        Task<List<Accommodation>> GetAllAccommodationsAsync();
        Task<Accommodation> GetAccommodationByIdAsync(int id);
        Task<List<Booking>> GetAllBookingsAsync();
        Task<Booking> GetBookingByIdAsync(int id);
        Task<List<Booking>> GetUserBookingsAsync(string userId);
        Task<Booking> CreateBookingAsync(Booking booking);
        Task UpdateBookingAsync(int id, Booking booking);
        Task DeleteBookingAsync(int id);
        Task<AuthResponse> LoginAsync(string email, string password, bool remember);
        Task<AuthResponse> RegisterAsync(RegisterRequest register);
        Task<bool> LogoutAsync();
        Task<User> GetCurrentUserAsync(ClaimsPrincipal userClaims);
        Task<bool> IsUserAdminAsync(ClaimsPrincipal userClaims);
        Task<User> GetUserAsync(string id);
        Task<string> UploadImageAsync(Stream imageStream, string fileName, string entityType, int entityId);
        Task<Accommodation> CreateAccommodationAsync(Accommodation accommodation);
        Task UpdateAccommodationAsync(int id, Accommodation accommodation);
        Task DeleteAccommodationAsync(int id);
        Task<List<Destination>> SearchDestinationsAsync(string query);
        Task<Destination> CreateDestinationAsync(Destination destination);
        Task UpdateDestinationAsync(int id, Destination destination);
        Task DeleteDestinationAsync(int id);
        Task<List<User>> GetAllUsersAsync();
        Task<User> CreateUserAsync(User user);
        Task UpdateUserAsync(string id, User user);
        Task DeleteUserAsync(string id);
    }
} 
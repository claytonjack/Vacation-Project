using VacationBooking.Models;

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
    }
} 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using VacationBooking.Models;
using VacationBooking.Services;
using System.Security.Claims;

namespace VacationBooking.Controllers
{
    /// <summary>
    /// Controller for managing vacation booking operations.
    /// Principal Author: Hillary
    /// </summary>
    [Authorize]
    public class BookingController : Controller
    {
        /// <summary>
        /// Service for interacting with the vacation API
        /// </summary>
        private readonly VacationApiService _apiService;
        
        /// <summary>
        /// Manager for user authentication and information
        /// </summary>
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Initializes a new instance of the BookingController
        /// </summary>
        /// <param name="apiService">Service for API interactions</param>
        /// <param name="userManager">Manager for user operations</param>
        public BookingController(VacationApiService apiService,
                              UserManager<User> userManager)
        {
            _apiService = apiService;
            _userManager = userManager;
        }

        /// <summary>
        /// Displays the form for creating a new booking
        /// </summary>
        /// <param name="id">The ID of the vacation package to book</param>
        /// <returns>The booking form view</returns>
        public async Task<IActionResult> NewBooking(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var vacation = await _apiService.GetVacationByIdAsync(id);
                
                if (vacation == null)
                {
                    return NotFound();
                }

                var booking = new Booking
                {
                    VacationID = vacation.VacationID,
                    UserID = user.Id,
                    CheckInDate = DateTime.Now.AddDays(1),
                    NumberOfNights = 1,
                    NumberOfGuests = 1,
                    Vacation = vacation
                };

                return View(booking);
            }
            catch (Exception)
            {
                return View("Error", new ErrorViewModel { RequestId = "Failed to get vacation details" });
            }
        }

        /// <summary>
        /// Processes the creation of a new booking
        /// </summary>
        /// <param name="vacationID">The ID of the vacation package to book</param>
        /// <param name="checkInDate">The check-in date</param>
        /// <param name="numberOfNights">Number of nights to stay</param>
        /// <param name="numberOfGuests">Number of guests</param>
        /// <param name="specialRequests">Any special requests</param>
        /// <returns>Redirect to confirmation page on success, or booking form with errors</returns>
        /// <author>Jack</author>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewBooking(int vacationID, DateTime checkInDate, int numberOfNights, int numberOfGuests, string specialRequests)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var vacation = await _apiService.GetVacationByIdAsync(vacationID);
                    
                if (vacation == null)
                {
                    return NotFound();
                }

                var booking = new Booking
                {
                    VacationID = vacationID,
                    UserID = user.Id,
                    CheckInDate = checkInDate,
                    NumberOfNights = numberOfNights,
                    NumberOfGuests = numberOfGuests,
                    SpecialRequests = specialRequests,
                    TotalPrice = vacation.PricePerNight * numberOfNights,
                    BookingDate = DateTime.Now
                };

                var createdBooking = await _apiService.CreateBookingAsync(booking);
                return RedirectToAction("Confirmation", new { id = createdBooking.BookingID });
            }
            catch (Exception ex)
            {
                try
                {
                    var vacation = await _apiService.GetVacationByIdAsync(vacationID);
                    
                    if (vacation == null)
                    {
                        return NotFound();
                    }
                    
                    var bookingForView = new Booking
                    {
                        VacationID = vacationID,
                        UserID = user.Id,
                        CheckInDate = checkInDate,
                        NumberOfNights = numberOfNights,
                        NumberOfGuests = numberOfGuests,
                        SpecialRequests = specialRequests,
                        Vacation = vacation
                    };
                    
                    ModelState.AddModelError("", $"Error creating booking: {ex.Message}");
                    return View(bookingForView);
                }
                catch
                {
                    return View("Error", new ErrorViewModel { RequestId = "Failed to create booking" });
                }
            }
        }

        /// <summary>
        /// Displays the booking confirmation page
        /// </summary>
        /// <param name="id">The ID of the booking to confirm</param>
        /// <returns>The confirmation view</returns>
        public async Task<IActionResult> Confirmation(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var booking = await _apiService.GetBookingByIdAsync(id);
                
                if (booking == null || booking.UserID != userId)
                {
                    return RedirectToAction("Index", "Home");
                }

                return View(booking);
            }
            catch (Exception)
            {
                return View("Error", new ErrorViewModel { RequestId = "Failed to get booking details" });
            }
        }
    }
}
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
        private readonly IVacationApiService _apiService;

        /// <summary>
        /// Initializes a new instance of the BookingController
        /// </summary>
        /// <param name="apiService">Service for API interactions</param>
        public BookingController(IVacationApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Displays the form for creating a new booking
        /// </summary>
        /// <param name="id">The ID of the vacation package to book</param>
        /// <returns>The booking form view</returns>
        public async Task<IActionResult> NewBooking(int id)
        {
            var user = await _apiService.GetCurrentUserAsync(User);
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
            var user = await _apiService.GetCurrentUserAsync(User);
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

                // Validate input
                if (checkInDate < DateTime.Today)
                {
                    ModelState.AddModelError("CheckInDate", "Check-in date cannot be in the past");
                    return View(new Booking
                    {
                        VacationID = vacationID,
                        UserID = user.Id,
                        CheckInDate = DateTime.Now.AddDays(1),
                        NumberOfNights = numberOfNights,
                        NumberOfGuests = numberOfGuests,
                        SpecialRequests = specialRequests,
                        Vacation = vacation
                    });
                }

                // Log all fields to help with debugging
                Console.WriteLine($"Creating booking with: VacationID={vacationID}, UserID={user.Id}, " +
                                  $"CheckInDate={checkInDate}, Nights={numberOfNights}, Guests={numberOfGuests}");
                
                // Create booking with only the required IDs, not the full objects
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

                // The associated User and Vacation objects are causing the problem
                // Let's remove any references to them before sending to the API
                
                var createdBooking = await _apiService.CreateBookingAsync(booking);
                return RedirectToAction("Confirmation", new { id = createdBooking.BookingID });
            }
            catch (Exception ex)
            {
                // More detailed error handling
                Console.WriteLine($"Booking creation error: {ex.Message}");
                
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
                
                if (booking == null)
                {
                    // Handle booking not found - don't try to set ShowRequestId
                    return View("Error", new ErrorViewModel { 
                        RequestId = "Booking not found"
                    });
                }
                
                if (booking.UserID != userId)
                {
                    // Handle unauthorized access
                    return RedirectToAction("Index", "Home");
                }

                // If the booking doesn't have vacation details, try to load them separately
                if (booking.Vacation == null && booking.VacationID > 0)
                {
                    var vacation = await _apiService.GetVacationByIdAsync(booking.VacationID);
                    booking.Vacation = vacation;
                }

                return View(booking);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Confirmation: {ex.Message}");
                
                // Create a more detailed error model - don't try to set ShowRequestId
                return View("Error", new ErrorViewModel { 
                    RequestId = $"Failed to get booking details: {ex.Message}"
                });
            }
        }
    }
}
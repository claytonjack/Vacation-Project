using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationBooking.Data;
using VacationBooking.Models;

namespace VacationBooking.Controllers
{
    /// <summary>
    /// Controller for managing booking operations in the vacation booking system.
    /// Principal Author: Hillary
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        /// <summary>
        /// Database context for the vacation booking system
        /// </summary>
        private readonly VacationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the BookingsController with the specified context
        /// </summary>
        /// <param name="context">The database context to be used</param>
        public BookingsController(VacationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all bookings in the system including related user and vacation information
        /// </summary>
        /// <returns>A list of all bookings</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Vacation)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a specific booking by ID
        /// </summary>
        /// <param name="id">The ID of the booking to retrieve</param>
        /// <returns>The requested booking if found</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Vacation)
                .FirstOrDefaultAsync(b => b.BookingID == id);

            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }

        /// <summary>
        /// Gets all bookings for a specific user
        /// </summary>
        /// <param name="userId">The ID of the user whose bookings to retrieve</param>
        /// <returns>A list of bookings for the specified user</returns>
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsByUser(string userId)
        {
            return await _context.Bookings
                .Include(b => b.Vacation)
                .Where(b => b.UserID == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new booking
        /// </summary>
        /// <param name="booking">The booking to create</param>
        /// <returns>The created booking</returns>
        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking(Booking booking)
        {
            // Validate basic required fields
            if (string.IsNullOrEmpty(booking.UserID) || booking.VacationID <= 0 || 
                booking.CheckInDate == default || booking.NumberOfNights <= 0 || booking.NumberOfGuests <= 0)
            {
                return BadRequest("Required booking information is missing");
            }

            // Clear navigation properties to avoid tracking conflicts
            booking.User = null;
            booking.Vacation = null;

            // Validate business rules
            if (booking.CheckInDate < DateTime.Today)
            {
                return BadRequest("Check-in date cannot be in the past");
            }

            // Check if user exists
            var user = await _context.Users.FindAsync(booking.UserID);
            if (user == null)
            {
                return BadRequest("Invalid User ID");
            }

            // Check if vacation exists and calculate total price
            var vacation = await _context.Vacations.FindAsync(booking.VacationID);
            if (vacation == null)
            {
                return BadRequest("Invalid Vacation ID");
            }

            booking.TotalPrice = vacation.PricePerNight * booking.NumberOfNights;
            booking.BookingDate = DateTime.Now;

            // Disable validation for this operation
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            
            try
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
            
                // Return a simplified booking without circular references
                return CreatedAtAction("GetBooking", new { id = booking.BookingID }, new
                {
                    booking.BookingID,
                    booking.UserID,
                    booking.VacationID,
                    booking.CheckInDate,
                    booking.NumberOfNights,
                    booking.NumberOfGuests,
                    booking.TotalPrice,
                    booking.BookingDate,
                    booking.SpecialRequests
                });
            }
            finally
            {
                _context.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }

        /// <summary>
        /// Updates an existing booking
        /// </summary>
        /// <param name="id">The ID of the booking to update</param>
        /// <param name="booking">The updated booking information</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(int id, Booking booking)
        {
            if (id != booking.BookingID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Recalculate total price if nights changed
            var existingBooking = await _context.Bookings.AsNoTracking().FirstOrDefaultAsync(b => b.BookingID == id);
            if (existingBooking == null)
            {
                return NotFound();
            }

            if (existingBooking.NumberOfNights != booking.NumberOfNights ||
                existingBooking.VacationID != booking.VacationID)
            {
                var vacation = await _context.Vacations.FindAsync(booking.VacationID);
                if (vacation == null)
                {
                    return BadRequest("Invalid Vacation ID");
                }
                booking.TotalPrice = vacation.PricePerNight * booking.NumberOfNights;
            }

            _context.Entry(booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a booking
        /// </summary>
        /// <param name="id">The ID of the booking to delete</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Checks if a booking exists
        /// </summary>
        /// <param name="id">The ID of the booking to check</param>
        /// <returns>True if the booking exists, false otherwise</returns>
        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingID == id);
        }
    }
}
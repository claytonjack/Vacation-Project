using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationBooking.Data;
using VacationBooking.Models;

namespace VacationBooking.Controllers
{
    /// <summary>
    /// Controller for managing accommodation operations in the vacation booking system.
    /// Principal Author: Mostafa
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccommodationsController : ControllerBase
    {
        /// <summary>
        /// Database context for the vacation booking system
        /// </summary>
        private readonly VacationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the AccommodationsController with the specified context
        /// </summary>
        /// <param name="context">The database context to be used</param>
        public AccommodationsController(VacationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all accommodations in the system
        /// </summary>
        /// <returns>A list of all accommodations</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Accommodation>>> GetAccommodations()
        {
            return await _context.Accommodations.ToListAsync();
        }

        /// <summary>
        /// Gets a specific accommodation by ID
        /// </summary>
        /// <param name="id">The ID of the accommodation to retrieve</param>
        /// <returns>The requested accommodation if found</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Accommodation>> GetAccommodation(int id)
        {
            var accommodation = await _context.Accommodations.FindAsync(id);

            if (accommodation == null)
            {
                return NotFound();
            }

            return accommodation;
        }

        /// <summary>
        /// Updates an existing accommodation
        /// </summary>
        /// <param name="id">The ID of the accommodation to update</param>
        /// <param name="accommodation">The updated accommodation information</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccommodation(int id, Accommodation accommodation)
        {
            if (id != accommodation.AccommodationID)
            {
                return BadRequest();
            }

            _context.Entry(accommodation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccommodationExists(id))
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
        /// Creates a new accommodation
        /// </summary>
        /// <param name="accommodation">The accommodation to create</param>
        /// <returns>The created accommodation</returns>
        [HttpPost]
        public async Task<ActionResult<Accommodation>> PostAccommodation(Accommodation accommodation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Accommodations.Add(accommodation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccommodation", new { id = accommodation.AccommodationID }, accommodation);
        }

        /// <summary>
        /// Deletes an accommodation
        /// </summary>
        /// <param name="id">The ID of the accommodation to delete</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccommodation(int id)
        {
            var accommodation = await _context.Accommodations.FindAsync(id);
            if (accommodation == null)
            {
                return NotFound();
            }

            _context.Accommodations.Remove(accommodation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Checks if an accommodation exists
        /// </summary>
        /// <param name="id">The ID of the accommodation to check</param>
        /// <returns>True if the accommodation exists, false otherwise</returns>
        private bool AccommodationExists(int id)
        {
            return _context.Accommodations.Any(e => e.AccommodationID == id);
        }
    }
}
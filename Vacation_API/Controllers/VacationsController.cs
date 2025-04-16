using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationBooking.Data;
using VacationBooking.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;

namespace VacationBooking.Controllers
{
    /// <summary>
    /// Controller for managing vacation package operations in the vacation booking system.
    /// Principal Author: Jack
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class VacationsController : ControllerBase
    {
        /// <summary>
        /// Database context for the vacation booking system
        /// </summary>
        private readonly VacationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the VacationsController with the specified context
        /// </summary>
        /// <param name="context">The database context to be used</param>
        public VacationsController(VacationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all vacation packages in the system including related destination and accommodation information
        /// </summary>
        /// <returns>A list of all vacation packages</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vacation>>> GetVacations()
        {
            return await _context.Vacations
                .Include(v => v.Destination)
                .Include(v => v.Accommodation)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a specific vacation package by ID
        /// </summary>
        /// <param name="id">The ID of the vacation package to retrieve</param>
        /// <returns>The requested vacation package if found</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Vacation>> GetVacation(int id)
        {
            var vacation = await _context.Vacations
                .Include(v => v.Destination)
                .Include(v => v.Accommodation)
                .FirstOrDefaultAsync(v => v.VacationID == id);

            if (vacation == null)
            {
                return NotFound();
            }

            return vacation;
        }

        /// <summary>
        /// Searches for vacation packages based on a query string
        /// </summary>
        /// <param name="query">The search query to filter vacation packages</param>
        /// <returns>A list of vacation packages matching the search criteria</returns>
        /// <author>Mostafa</author>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Vacation>>> SearchVacations([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty");
            }

            return await _context.Vacations
                .Include(v => v.Destination)
                .Include(v => v.Accommodation)
                .Where(v => v.Name.Contains(query) ||
                           v.Description.Contains(query) ||
                           v.Destination.City.Contains(query) ||
                           v.Destination.Country.Contains(query))
                .ToListAsync();
        }

        /// <summary>
        /// Gets all vacation packages for a specific destination
        /// </summary>
        /// <param name="destinationId">The ID of the destination to filter vacation packages</param>
        /// <returns>A list of vacation packages for the specified destination</returns>
        [HttpGet("destination/{destinationId}")]
        public async Task<ActionResult<IEnumerable<Vacation>>> GetVacationsByDestination(int destinationId)
        {
            return await _context.Vacations
                .Include(v => v.Destination)
                .Include(v => v.Accommodation)
                .Where(v => v.DestinationID == destinationId)
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new vacation package
        /// </summary>
        /// <param name="vacation">The vacation package to create</param>
        /// <returns>The created vacation package</returns>
        [HttpPost]
        public async Task<ActionResult<Vacation>> PostVacation(Vacation vacation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify destination exists
            var destinationExists = await _context.Destinations.AnyAsync(d => d.DestinationID == vacation.DestinationID);
            if (!destinationExists)
            {
                return BadRequest("Invalid Destination ID");
            }

            // Verify accommodation exists
            var accommodationExists = await _context.Accommodations.AnyAsync(a => a.AccommodationID == vacation.AccommodationID);
            if (!accommodationExists)
            {
                return BadRequest("Invalid Accommodation ID");
            }

            _context.Vacations.Add(vacation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVacation", new { id = vacation.VacationID }, vacation);
        }

        /// <summary>
        /// Updates an existing vacation package
        /// </summary>
        /// <param name="id">The ID of the vacation package to update</param>
        /// <param name="vacation">The updated vacation package information</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVacation(int id, Vacation vacation)
        {
            if (id != vacation.VacationID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify destination exists
            var destinationExists = await _context.Destinations.AnyAsync(d => d.DestinationID == vacation.DestinationID);
            if (!destinationExists)
            {
                return BadRequest("Invalid Destination ID");
            }

            // Verify accommodation exists
            var accommodationExists = await _context.Accommodations.AnyAsync(a => a.AccommodationID == vacation.AccommodationID);
            if (!accommodationExists)
            {
                return BadRequest("Invalid Accommodation ID");
            }

            _context.Entry(vacation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VacationExists(id))
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
        /// Deletes a vacation package
        /// </summary>
        /// <param name="id">The ID of the vacation package to delete</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVacation(int id)
        {
            var vacation = await _context.Vacations.FindAsync(id);
            if (vacation == null)
            {
                return NotFound();
            }

            // Check if vacation has bookings
            var hasBookings = await _context.Bookings.AnyAsync(b => b.VacationID == id);
            if (hasBookings)
            {
                return BadRequest("Cannot delete vacation with existing bookings");
            }

            _context.Vacations.Remove(vacation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Checks if a vacation package exists
        /// </summary>
        /// <param name="id">The ID of the vacation package to check</param>
        /// <returns>True if the vacation package exists, false otherwise</returns>
        private bool VacationExists(int id)
        {
            return _context.Vacations.Any(e => e.VacationID == id);
        }

        [HttpPost("{id}/upload-image")]
        public async Task<ActionResult<string>> UploadVacationImage(int id, IFormFile file)
        {
            var vacation = await _context.Vacations.FindAsync(id);
            
            if (vacation == null)
            {
                return NotFound();
            }
            
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded");
            }
            
            try
            {
                // Save to the existing images/vacation directory
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "images", "vacation");
                
                // Generate unique filename
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                string filePath = Path.Combine(uploadsFolder, fileName);
                
                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                
                // Update vacation with new image URL
                string imageUrl = $"/images/vacation/{fileName}";
                vacation.ImageUrl = imageUrl;
                
                _context.Entry(vacation).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                
                return imageUrl;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading image: {ex.Message}");
            }
        }
    }
}
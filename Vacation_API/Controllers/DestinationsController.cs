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
    /// Controller for managing destination operations in the vacation booking system.
    /// Principal Author: Mostafa
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DestinationsController : ControllerBase
    {
        /// <summary>
        /// Database context for the vacation booking system
        /// </summary>
        private readonly VacationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the DestinationsController with the specified context
        /// </summary>
        /// <param name="context">The database context to be used</param>
        public DestinationsController(VacationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all destinations in the system
        /// </summary>
        /// <returns>A list of all destinations</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Destination>>> GetDestinations()
        {
            return await _context.Destinations.ToListAsync();
        }

        /// <summary>
        /// Gets a specific destination by ID
        /// </summary>
        /// <param name="id">The ID of the destination to retrieve</param>
        /// <returns>The requested destination if found</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Destination>> GetDestination(int id)
        {
            var destination = await _context.Destinations.FindAsync(id);

            if (destination == null)
            {
                return NotFound();
            }

            return destination;
        }

        /// <summary>
        /// Searches for destinations based on a query string
        /// </summary>
        /// <param name="query">The search query to filter destinations</param>
        /// <returns>A list of destinations matching the search criteria</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Destination>>> SearchDestinations([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty");
            }

            return await _context.Destinations
                .Where(d => d.City.Contains(query) || d.Country.Contains(query))
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new destination
        /// </summary>
        /// <param name="destination">The destination to create</param>
        /// <returns>The created destination</returns>
        [HttpPost]
        public async Task<ActionResult<Destination>> PostDestination(Destination destination)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if destination already exists
            var exists = await _context.Destinations
                .AnyAsync(d => d.City == destination.City && d.Country == destination.Country);

            if (exists)
            {
                return Conflict("This city/country combination already exists");
            }

            _context.Destinations.Add(destination);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDestination", new { id = destination.DestinationID }, destination);
        }

        /// <summary>
        /// Updates an existing destination
        /// </summary>
        /// <param name="id">The ID of the destination to update</param>
        /// <param name="destination">The updated destination information</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDestination(int id, Destination destination)
        {
            if (id != destination.DestinationID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if updated destination would conflict with existing one
            var exists = await _context.Destinations
                .AnyAsync(d => d.DestinationID != id &&
                              d.City == destination.City &&
                              d.Country == destination.Country);

            if (exists)
            {
                return Conflict("This city/country combination already exists");
            }

            _context.Entry(destination).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DestinationExists(id))
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
        /// Deletes a destination
        /// </summary>
        /// <param name="id">The ID of the destination to delete</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDestination(int id)
        {
            var destination = await _context.Destinations.FindAsync(id);
            if (destination == null)
            {
                return NotFound();
            }

            // Check if destination is used in any vacations
            var hasVacations = await _context.Vacations
                .AnyAsync(v => v.DestinationID == id);

            if (hasVacations)
            {
                return BadRequest("Cannot delete destination as it is associated with existing vacations");
            }

            _context.Destinations.Remove(destination);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Checks if a destination exists
        /// </summary>
        /// <param name="id">The ID of the destination to check</param>
        /// <returns>True if the destination exists, false otherwise</returns>
        private bool DestinationExists(int id)
        {
            return _context.Destinations.Any(e => e.DestinationID == id);
        }

        /// <summary>
        /// Uploads an image for a destination
        /// </summary>
        /// <param name="id">The ID of the destination to update</param>
        /// <param name="file">The image file to upload</param>
        /// <returns>The URL of the uploaded image</returns>
        [HttpPost("{id}/upload-image")]
        public async Task<ActionResult<string>> UploadDestinationImage(int id, IFormFile file)
        {
            var destination = await _context.Destinations.FindAsync(id);
            
            if (destination == null)
            {
                return NotFound();
            }
            
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded");
            }
            
            try
            {
                // Save to the existing images/destination directory
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "images", "destination");
                
                // Generate unique filename
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                string filePath = Path.Combine(uploadsFolder, fileName);
                
                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                
                // Update destination with new image URL (relative path)
                string imageUrl = $"/images/destination/{fileName}";
                destination.ImageUrl = imageUrl;
                
                _context.Entry(destination).State = EntityState.Modified;
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
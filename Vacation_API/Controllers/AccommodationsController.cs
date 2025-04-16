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

        /// <summary>
        /// Uploads an image for an accommodation
        /// </summary>
        /// <param name="id">The ID of the accommodation to update</param>
        /// <param name="file">The image file to upload</param>
        /// <returns>The URL of the uploaded image</returns>
        [HttpPost("{id}/upload-image")]
        public async Task<ActionResult<string>> UploadAccommodationImage(int id, IFormFile file)
        {
            var accommodation = await _context.Accommodations.FindAsync(id);
            
            if (accommodation == null)
            {
                return NotFound();
            }
            
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded");
            }
            
            try
            {
                // Save to the existing images/accommodation directory
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "images", "accommodation");
                
                // Generate unique filename
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                string filePath = Path.Combine(uploadsFolder, fileName);
                
                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                
                // Update accommodation with new image URL (relative path)
                string imageUrl = $"/images/accommodation/{fileName}";
                accommodation.ImageUrl = imageUrl;
                
                _context.Entry(accommodation).State = EntityState.Modified;
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
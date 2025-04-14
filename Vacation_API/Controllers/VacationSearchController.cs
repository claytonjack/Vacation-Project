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
    /// Controller for searching vacation packages with advanced search criteria.
    /// Principal Author: Mostafa 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class VacationSearchController : ControllerBase
    {
        /// <summary>
        /// Database context for the vacation booking system
        /// </summary>
        private readonly VacationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the VacationSearchController with the specified context
        /// </summary>
        /// <param name="context">The database context to be used</param>
        public VacationSearchController(VacationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Searches for vacation packages based on multiple criteria
        /// </summary>
        /// <param name="criteria">The search criteria to filter vacation packages</param>
        /// <returns>A list of vacation packages matching the search criteria</returns>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Vacation>>> SearchVacations([FromBody] SearchCriteria criteria)
        {
            try
            {
                // Start with base query including related entities
                var query = _context.Vacations
                    .Include(v => v.Destination)
                    .Include(v => v.Accommodation)
                    .AsQueryable();

                // Apply filters based on provided criteria
                if (!string.IsNullOrEmpty(criteria.City))
                {
                    query = query.Where(v => v.Destination.City.Contains(criteria.City));
                }

                if (!string.IsNullOrEmpty(criteria.Country))
                {
                    query = query.Where(v => v.Destination.Country.Contains(criteria.Country));
                }

                if (criteria.MaxPricePerNight > 0)
                {
                    query = query.Where(v => v.PricePerNight <= criteria.MaxPricePerNight);
                }

                if (!string.IsNullOrEmpty(criteria.RoomType))
                {
                    query = query.Where(v => v.Accommodation.RoomType == criteria.RoomType);
                }

                if (criteria.AllInclusive.HasValue)
                {
                    query = query.Where(v => v.AllInclusive == criteria.AllInclusive.Value);
                }

                // Execute the query and return results
                var results = await query.ToListAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while searching vacations: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all available filter options for the search form
        /// </summary>
        /// <returns>An object containing all available filter options</returns>
        [HttpGet("GetFilterOptions")]
        public async Task<ActionResult<object>> GetFilterOptions()
        {
            try
            {
                var cities = await _context.Destinations
                    .Select(d => d.City)
                    .Distinct()
                    .ToListAsync();

                var countries = await _context.Destinations
                    .Select(d => d.Country)
                    .Distinct()
                    .ToListAsync();

                var roomTypes = await _context.Accommodations
                    .Select(a => a.RoomType)
                    .Distinct()
                    .ToListAsync();

                var maxPrice = await _context.Vacations
                    .MaxAsync(v => v.PricePerNight);

                return Ok(new
                {
                    Cities = cities,
                    Countries = countries,
                    RoomTypes = roomTypes,
                    MaxPrice = maxPrice
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while getting filter options: {ex.Message}");
            }
        }
    }
}
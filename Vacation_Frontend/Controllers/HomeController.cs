using Microsoft.AspNetCore.Mvc;
using VacationBooking.Models;
using VacationBooking.Services;
using System.Diagnostics;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VacationBooking.Controllers
{
    /// <summary>
    /// Controller for the main home page and general website navigation.
    /// Principal Author: Mostafa
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Service for interacting with the vacation API
        /// </summary>
        private readonly VacationApiService _apiService;

        /// <summary>
        /// Initializes a new instance of the HomeController
        /// </summary>
        /// <param name="apiService">Service for API interactions</param>
        public HomeController(VacationApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Displays the home page with featured vacation packages
        /// </summary>
        /// <returns>The home page view with featured vacations</returns>
        public async Task<IActionResult> Index()
        {
            try
            {
                // Get vacations from API
                var allVacations = await _apiService.GetAllVacationsAsync();
                
                // Take 3 random vacations for the featured section
                var featuredVacations = allVacations
                    .OrderBy(v => Guid.NewGuid())
                    .Take(3)
                    .ToList();
                
                // Send to view
                return View(featuredVacations);
            }
            catch (Exception)
            {
                return View(new List<Vacation>());
            }
        }

        /// <summary>
        /// Displays the privacy policy page
        /// </summary>
        /// <returns>The privacy policy view</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Displays the error page
        /// </summary>
        /// <returns>The error view with request ID information</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
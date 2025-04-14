using Microsoft.AspNetCore.Mvc;
using VacationBooking.Models;
using VacationBooking.Services;
using System;
using System.Threading.Tasks;

namespace VacationBooking.Controllers
{
    /// <summary>
    /// Controller for vacation package search and browsing.
    /// Principal Author: Jack
    /// </summary>
    public class VacationController : Controller
    {
        /// <summary>
        /// Service for interacting with the vacation API
        /// </summary>
        private readonly IVacationApiService _apiService;

        /// <summary>
        /// Initializes a new instance of the VacationController
        /// </summary>
        /// <param name="apiService">Service for API interactions</param>
        public VacationController(IVacationApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Displays the vacation search form
        /// </summary>
        /// <returns>The search view with empty search criteria</returns>
        public IActionResult Search()
        {
            return View(new SearchCriteria());
        }

        /// <summary>
        /// Processes a vacation search and displays the results
        /// </summary>
        /// <param name="criteria">The search criteria</param>
        /// <returns>The search results view</returns>
        [HttpPost]
        public async Task<IActionResult> Results(SearchCriteria criteria)
        {
            try {
                var results = await _apiService.SearchVacationsAsync(criteria);
                
                ViewBag.SearchCriteria = criteria;
                return View(results);
            }
            catch (Exception) {
                return View("Error", new ErrorViewModel { RequestId = "Failed to search vacations" });
            }
        }

        /// <summary>
        /// Displays the details of a specific vacation package
        /// </summary>
        /// <param name="id">The ID of the vacation package to display</param>
        /// <returns>The vacation details view</returns>
        /// <author>Hillary</author>
        public async Task<IActionResult> Details(int id)
        {
            try {
                var vacation = await _apiService.GetVacationByIdAsync(id);
                
                if (vacation == null)
                {
                    return NotFound();
                }

                return View(vacation);
            }
            catch (Exception) {
                return View("Error", new ErrorViewModel { RequestId = "Failed to get vacation details" });
            }
        }
    }
}
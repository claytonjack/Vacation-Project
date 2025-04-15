using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using VacationBooking.Models;
using VacationBooking.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace VacationBooking.Controllers
{
    /// <summary>
    /// Controller for administrative operations including vacation package management.
    /// Principal Author: Mostafa
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        /// <summary>
        /// Service for interacting with the vacation API
        /// </summary>
        private readonly IVacationApiService _apiService;

        /// <summary>
        /// Initializes a new instance of the AdminController
        /// </summary>
        /// <param name="apiService">Service for API interactions</param>
        public AdminController(IVacationApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Displays the admin dashboard with vacation listings
        /// </summary>
        /// <returns>The dashboard view with vacation packages</returns>
        public async Task<IActionResult> Dashboard()
        {
            if (!await IsUserAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var vacations = await _apiService.GetAllVacationsAsync();
                return View(vacations);
            }
            catch (Exception)
            {
                return View(new List<Vacation>());
            }
        }

        /// <summary>
        /// Displays the form for creating a new vacation package
        /// </summary>
        /// <returns>The create vacation view</returns>
        public async Task<IActionResult> Create()
        {
            if (!await IsUserAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                ViewBag.Destinations = await _apiService.GetAllDestinationsAsync();
                ViewBag.Accommodations = await _apiService.GetAllAccommodationsAsync();
                
                var vacation = new Vacation();
                
                return View(vacation);
            }
            catch (Exception)
            {
                return View("Error", new ErrorViewModel { RequestId = "Failed to load destinations/accommodations" });
            }
        }

        /// <summary>
        /// Processes the creation of a new vacation package
        /// </summary>
        /// <param name="vacation">The vacation package to create</param>
        /// <param name="vacationImage">The main image for the vacation package</param>
        /// <param name="destinationImage">The image for the destination</param>
        /// <param name="accommodationImage">The image for the accommodation</param>
        /// <returns>Redirect to dashboard on success, or create form with errors</returns>
        /// <author>Hillary</author>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vacation vacation, IFormFile vacationImage, IFormFile destinationImage, IFormFile accommodationImage)
        {
            if (!await IsUserAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            ModelState.Remove("Destination");
            ModelState.Remove("Accommodation");
            ModelState.Remove("Bookings");
            ModelState.Remove("vacationImage");
            ModelState.Remove("destinationImage");
            ModelState.Remove("accommodationImage");

            if (vacation.DestinationID <= 0)
            {
                ModelState.AddModelError("DestinationID", "Please select a destination");
            }
            
            if (vacation.AccommodationID <= 0)
            {
                ModelState.AddModelError("AccommodationID", "Please select an accommodation");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (vacationImage != null && vacationImage.Length > 0)
                    {
                        vacation.ImageUrl = SaveImage(vacationImage, "vacations");
                    }

                    var createdVacation = await _apiService.CreateVacationAsync(vacation);
                    
                    TempData["Message"] = $"Vacation package '{vacation.Name}' was created successfully.";
                    return RedirectToAction("Dashboard");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating vacation: {ex.Message}");
                }
            }

            try
            {
                ViewBag.Destinations = await _apiService.GetAllDestinationsAsync();
                ViewBag.Accommodations = await _apiService.GetAllAccommodationsAsync();
            }
            catch
            {
                ViewBag.Destinations = new List<Destination>();
                ViewBag.Accommodations = new List<Accommodation>();
            }
            
            return View(vacation);
        }

        /// <summary>
        /// Displays the form for editing an existing vacation package
        /// </summary>
        /// <param name="id">The ID of the vacation package to edit</param>
        /// <returns>The edit vacation view</returns>
        public async Task<IActionResult> Edit(int id)
        {
            if (!await IsUserAdmin())
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

                ViewBag.Destinations = await _apiService.GetAllDestinationsAsync();
                ViewBag.Accommodations = await _apiService.GetAllAccommodationsAsync();
                return View(vacation);
            }
            catch (Exception)
            {
                return View("Error", new ErrorViewModel { RequestId = "Failed to load vacation details" });
            }
        }

        /// <summary>
        /// Processes the update of an existing vacation package
        /// </summary>
        /// <param name="id">The ID of the vacation package to update</param>
        /// <param name="vacation">The updated vacation package information</param>
        /// <param name="photoFile">New image file for the vacation package</param>
        /// <param name="deleteVacationImage">Whether to delete the existing image</param>
        /// <returns>Redirect to dashboard on success, or edit form with errors</returns>
        /// <author>Hillary</author>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vacation vacation, IFormFile photoFile, bool deleteVacationImage = false)
        {
            if (!await IsUserAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != vacation.VacationID)
            {
                return NotFound();
            }

            ModelState.Remove("Destination");
            ModelState.Remove("Accommodation");
            ModelState.Remove("Bookings");
            ModelState.Remove("photoFile");

            if (ModelState.IsValid)
            {
                try
                {
                    if (photoFile != null && photoFile.Length > 0)
                    {
                        vacation.ImageUrl = SaveImage(photoFile, "vacations");
                    }
                    else if (deleteVacationImage)
                    {
                        vacation.ImageUrl = null;
                    }
                    
                    await _apiService.UpdateVacationAsync(id, vacation);
                    
                    TempData["Message"] = $"Vacation package '{vacation.Name}' was updated successfully.";
                    return RedirectToAction("Dashboard");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating vacation: {ex.Message}");
                }
            }

            try
            {
                ViewBag.Destinations = await _apiService.GetAllDestinationsAsync();
                ViewBag.Accommodations = await _apiService.GetAllAccommodationsAsync();
            }
            catch
            {
                ViewBag.Destinations = new List<Destination>();
                ViewBag.Accommodations = new List<Accommodation>();
            }
            
            return View(vacation);
        }

        /// <summary>
        /// Displays the confirmation page for deleting a vacation package
        /// </summary>
        /// <param name="id">The ID of the vacation package to delete</param>
        /// <returns>The delete confirmation view</returns>
        public async Task<IActionResult> Delete(int id)
        {
            if (!await IsUserAdmin())
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

                return View(vacation);
            }
            catch (Exception)
            {
                return View("Error", new ErrorViewModel { RequestId = "Failed to load vacation details" });
            }
        }

        /// <summary>
        /// Processes the deletion of a vacation package
        /// </summary>
        /// <param name="id">The ID of the vacation package to delete</param>
        /// <returns>Redirect to dashboard</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await IsUserAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                await _apiService.DeleteVacationAsync(id);
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting vacation: {ex.Message}";
                return RedirectToAction("Dashboard");
            }
        }

        /// <summary>
        /// Displays the photo management page for a vacation package
        /// </summary>
        /// <param name="id">The ID of the vacation package</param>
        /// <returns>The photo management view</returns>
        public async Task<IActionResult> ManagePhotos(int id)
        {
            if (!await IsUserAdmin())
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

                return View(vacation);
            }
            catch (Exception)
            {
                return View("Error", new ErrorViewModel { RequestId = "Failed to load vacation details" });
            }
        }

        /// <summary>
        /// Adds a photo to a vacation package
        /// </summary>
        /// <param name="vacationId">The ID of the vacation package</param>
        /// <param name="photoFile">The photo file to upload</param>
        /// <param name="caption">Caption for the photo</param>
        /// <returns>Redirect to photo management page</returns>
        /// <author>Hillary</author>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPhoto(int vacationId, IFormFile photoFile, string caption)
        {
            if (!await IsUserAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var vacation = await _apiService.GetVacationByIdAsync(vacationId);

                if (vacation == null)
                {
                    return NotFound();
                }

                if (photoFile != null && photoFile.Length > 0)
                {
                    vacation.ImageUrl = SaveImage(photoFile, "vacations");
                    await _apiService.UpdateVacationAsync(vacationId, vacation);
                    
                    TempData["Message"] = "Photo added successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No file was uploaded.";
                }

                return RedirectToAction("ManagePhotos", new { id = vacationId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error uploading photo: {ex.Message}";
                return RedirectToAction("ManagePhotos", new { id = vacationId });
            }
        }

        /// <summary>
        /// Deletes a photo from a vacation package
        /// </summary>
        /// <param name="vacationId">The ID of the vacation package</param>
        /// <param name="returnToEdit">Whether to return to the edit page</param>
        /// <returns>Redirect to dashboard or edit page based on parameter</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePhoto(int vacationId, int returnToEdit = 0)
        {
            if (!await IsUserAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var vacation = await _apiService.GetVacationByIdAsync(vacationId);

                if (vacation == null)
                {
                    return NotFound();
                }

                vacation.ImageUrl = null;
                await _apiService.UpdateVacationAsync(vacationId, vacation);

                TempData["Message"] = "Photo deleted successfully.";
                
                if (returnToEdit == 1)
                {
                    return RedirectToAction("Edit", new { id = vacationId });
                }
                
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting photo: {ex.Message}";
                return RedirectToAction("Dashboard");
            }
        }

        /// <summary>
        /// Checks if the current user is an administrator
        /// </summary>
        /// <returns>True if the user is an admin, false otherwise</returns>
        private async Task<bool> IsUserAdmin()
        {
            return await _apiService.IsUserAdminAsync(User);
        }

        /// <summary>
        /// Saves an image file to the server
        /// </summary>
        /// <param name="file">The image file to save</param>
        /// <param name="folderName">The folder to save the image in</param>
        /// <returns>The URL path to the saved image</returns>
        /// <author>Jack</author>
        private string SaveImage(IFormFile file, string folderName)
        {
            if (file != null && file.Length > 0)
            {
                if (file.ContentType.StartsWith("image/"))
                {
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folderName);
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    
                    string filePath = Path.Combine(uploadsFolder, fileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    
                    return $"/images/{folderName}/{fileName}";
                }
            }
            return null;
        }
    }
}
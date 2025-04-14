using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using VacationBooking.Models;
using VacationBooking.Services;
using System.Collections.Generic;

namespace VacationBooking.Controllers
{
    /// <summary>
    /// Controller for managing user accounts, authentication, and profile management.
    /// Principal Author: Hillary
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// Service for interacting with the vacation API
        /// </summary>
        private readonly VacationApiService _apiService;
        
        /// <summary>
        /// Manager for user authentication and information
        /// </summary>
        private readonly UserManager<User> _userManager;
        
        /// <summary>
        /// Manager for handling user sign-in operations
        /// </summary>
        private readonly SignInManager<User> _signInManager;

        /// <summary>
        /// Initializes a new instance of the AccountController
        /// </summary>
        /// <param name="apiService">Service for API interactions</param>
        /// <param name="userManager">Manager for user operations</param>
        /// <param name="signInManager">Manager for sign-in operations</param>
        public AccountController(VacationApiService apiService, 
                               UserManager<User> userManager,
                               SignInManager<User> signInManager)
        {
            _apiService = apiService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Displays the user registration form
        /// </summary>
        /// <returns>The registration view</returns>
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Processes the user registration form submission
        /// </summary>
        /// <param name="model">The user registration data</param>
        /// <returns>Redirect to home page on success, or registration form with errors</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email is already in use");
                    return View(model);
                }

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    IsAdmin = false
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("IsAdmin", user.IsAdmin.ToString()));
                    
                    if (user.IsAdmin)
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        /// <summary>
        /// Displays the login form
        /// </summary>
        /// <param name="returnUrl">URL to redirect to after successful login</param>
        /// <returns>The login view</returns>
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            Login login = new Login();
            login.ReturnUrl = returnUrl ?? Url.Content("~/");
            return View(login);
        }

        /// <summary>
        /// Processes the login form submission
        /// </summary>
        /// <param name="login">The login credentials</param>
        /// <returns>Redirect to requested page on success, or login form with errors</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(login.Email);
                if (user != null)
                {
                    await _signInManager.SignOutAsync();
                    var result = await _signInManager.PasswordSignInAsync(
                        user, login.Password, login.Remember, false);
                        
                    if (result.Succeeded)
                    {
                        if (string.IsNullOrEmpty(login.ReturnUrl))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        return Redirect(login.ReturnUrl);
                    }
                }
                
                ModelState.AddModelError("", "Invalid login attempt");
            }
            return View(login);
        }

        /// <summary>
        /// Displays the user profile page with booking history
        /// </summary>
        /// <returns>The profile view with user information and bookings</returns>
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                var bookings = await _apiService.GetUserBookingsAsync(user.Id);
                ViewBag.Bookings = bookings;

                return View(user);
            }
            catch (Exception)
            {
                ViewBag.Bookings = new List<Booking>();
                return View(user);
            }
        }

        /// <summary>
        /// Logs the current user out
        /// </summary>
        /// <returns>Redirect to home page</returns>
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
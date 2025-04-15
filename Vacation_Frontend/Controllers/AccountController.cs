using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using VacationBooking.Models;
using VacationBooking.Services;
using System.Collections.Generic;
using System.Security.Claims;

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
        private readonly IVacationApiService _apiService;

        /// <summary>
        /// Initializes a new instance of the AccountController
        /// </summary>
        /// <param name="apiService">Service for API interactions</param>
        public AccountController(IVacationApiService apiService)
        {
            _apiService = apiService;
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
                var registerRequest = new RegisterRequest
                {
                    Email = model.Email,
                    Password = model.Password,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address
                };

                var result = await _apiService.RegisterAsync(registerRequest);

                if (result.Success)
                {
                    // Create the cookie-based authentication
                    await CreateUserSession(result);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
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
                var result = await _apiService.LoginAsync(login.Email, login.Password, login.Remember);
                
                if (result.Success)
                {
                    // Create the cookie-based authentication
                    await CreateUserSession(result);
                    
                    if (string.IsNullOrEmpty(login.ReturnUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    return Redirect(login.ReturnUrl);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            try
            {
                var user = await _apiService.GetUserAsync(userId);
                var bookings = await _apiService.GetUserBookingsAsync(userId);
                ViewBag.Bookings = bookings;

                return View(user);
            }
            catch (Exception)
            {
                ViewBag.Bookings = new List<Booking>();
                return View(new User { Id = userId });
            }
        }

        /// <summary>
        /// Logs the current user out
        /// </summary>
        /// <returns>Redirect to home page</returns>
        public async Task<IActionResult> Logout()
        {
            // Sign out from API
            await _apiService.LogoutAsync();
            
            // Sign out locally
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        
        private async Task CreateUserSession(AuthResponse authResult)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, authResult.UserId),
                new Claim(ClaimTypes.Name, authResult.Email),
                new Claim(ClaimTypes.Email, authResult.Email),
                new Claim("FirstName", authResult.FirstName),
                new Claim("LastName", authResult.LastName),
                new Claim("IsAdmin", authResult.IsAdmin.ToString())
            };
            
            // Add roles
            foreach (var role in authResult.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
                
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };
            
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}
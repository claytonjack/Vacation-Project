using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VacationBooking.Models;

namespace VacationBooking.Controllers
{
    /// <summary>
    /// Controller for managing user accounts and authentication in the vacation booking system.
    /// Principal Author: Hillary
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        /// <summary>
        /// Manages user operations such as creation and authentication
        /// </summary>
        private readonly UserManager<User> _userManager;
        
        /// <summary>
        /// Manages user sign-in operations
        /// </summary>
        private readonly SignInManager<User> _signInManager;
        
        /// <summary>
        /// Manages role operations in the system
        /// </summary>
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>
        /// Initializes a new instance of the AccountsController with the specified managers
        /// </summary>
        /// <param name="userManager">The user manager for user operations</param>
        /// <param name="signInManager">The sign-in manager for authentication operations</param>
        /// <param name="roleManager">The role manager for role operations</param>
        public AccountsController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Registers a new user in the system
        /// </summary>
        /// <param name="model">The registration information</param>
        /// <returns>Authentication response with user details if successful</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest(new AuthResponse { 
                    Success = false, 
                    Errors = new List<string> { "Email is already in use" } 
                });
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
                await _userManager.AddToRoleAsync(user, "User");

                return Ok(new AuthResponse
                {
                    Success = true,
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsAdmin = user.IsAdmin,
                    Roles = (await _userManager.GetRolesAsync(user)).ToList()
                });
            }

            return BadRequest(new AuthResponse
            {
                Success = false,
                Errors = result.Errors.Select(e => e.Description).ToList()
            });
        }

        /// <summary>
        /// Authenticates a user and creates a session
        /// </summary>
        /// <param name="model">The login credentials</param>
        /// <returns>Authentication response with user details if successful</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Errors = new List<string> { "Invalid login attempt" }
                });
            }

            var result = await _signInManager.PasswordSignInAsync(
                user, model.Password, model.Remember, false);

            if (result.Succeeded)
            {
                return Ok(new AuthResponse
                {
                    Success = true,
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsAdmin = user.IsAdmin,
                    Roles = (await _userManager.GetRolesAsync(user)).ToList()
                });
            }

            return Unauthorized(new AuthResponse
            {
                Success = false,
                Errors = new List<string> { "Invalid login attempt" }
            });
        }
        
        /// <summary>
        /// Signs out the currently logged in user
        /// </summary>
        /// <returns>Success response</returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { Success = true });
        }
        
        /// <summary>
        /// Gets a user's profile information by ID
        /// </summary>
        /// <param name="id">The ID of the user to retrieve</param>
        /// <returns>The user's profile information if found</returns>
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            
            if (user == null)
            {
                return NotFound();
            }
            
            return Ok(new {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                IsAdmin = user.IsAdmin,
                Roles = await _userManager.GetRolesAsync(user)
            });
        }
    }
}

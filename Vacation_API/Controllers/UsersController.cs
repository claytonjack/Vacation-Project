using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationBooking.Data;
using VacationBooking.Models;

namespace VacationBooking.Controllers
{
    /// <summary>
    /// Controller for managing user operations in the vacation booking system.
    /// Principal Author: Hillary
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        /// <summary>
        /// User manager for managing user authentication and information
        /// </summary>
        private readonly UserManager<User> _userManager;
        
        /// <summary>
        /// Database context for the vacation booking system
        /// </summary>
        private readonly VacationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the UsersController with the specified user manager and context
        /// </summary>
        /// <param name="userManager">The identity user manager to be used</param>
        /// <param name="context">The database context to be used</param>
        public UsersController(UserManager<User> userManager, VacationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        /// Gets all users in the system with sensitive information excluded
        /// </summary>
        /// <returns>A list of all users</returns>
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            // Exclude sensitive information
            return _userManager.Users
                .Select(u => new User
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Address = u.Address,
                    IsAdmin = u.IsAdmin
                })
                .ToList();
        }

        /// <summary>
        /// Gets a specific user by ID
        /// </summary>
        /// <param name="id">The ID of the user to retrieve</param>
        /// <returns>The requested user if found</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Return user without sensitive data
            return new User
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                IsAdmin = user.IsAdmin
            };
        }

        /// <summary>
        /// Gets all bookings for a specific user
        /// </summary>
        /// <param name="id">The ID of the user whose bookings to retrieve</param>
        /// <returns>A list of bookings for the specified user</returns>
        [HttpGet("{id}/bookings")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetUserBookings(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return await _context.Bookings
                .Include(b => b.Vacation)
                .Where(b => b.UserID == id)
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user">The user to create</param>
        /// <returns>The created user</returns>
        [HttpPost]
        public async Task<ActionResult<User>> PostUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Password is required");
            }

            var newUser = new User
            {
                UserName = user.Email,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                IsAdmin = user.IsAdmin
            };

            var result = await _userManager.CreateAsync(newUser, user.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Return user without sensitive data
            return CreatedAtAction("GetUser", new { id = newUser.Id }, new User
            {
                Id = newUser.Id,
                UserName = newUser.UserName,
                Email = newUser.Email,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Address = newUser.Address,
                IsAdmin = newUser.IsAdmin
            });
        }

        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="id">The ID of the user to update</param>
        /// <param name="user">The updated user information</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, [FromBody] User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Address = user.Address;
            existingUser.Email = user.Email;
            existingUser.UserName = user.Email;
            existingUser.IsAdmin = user.IsAdmin;

            var result = await _userManager.UpdateAsync(existingUser);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Update password if provided
            if (!string.IsNullOrEmpty(user.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
                var passwordResult = await _userManager.ResetPasswordAsync(existingUser, token, user.Password);

                if (!passwordResult.Succeeded)
                {
                    return BadRequest(passwordResult.Errors);
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="id">The ID of the user to delete</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Check if user has bookings
            var hasBookings = await _context.Bookings.AnyAsync(b => b.UserID == id);
            if (hasBookings)
            {
                return BadRequest("Cannot delete user with existing bookings");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }
    }
}
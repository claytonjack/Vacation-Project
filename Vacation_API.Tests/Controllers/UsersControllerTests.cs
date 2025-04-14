using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationBooking.Controllers;
using VacationBooking.Data;
using VacationBooking.Models;
using Xunit;

namespace Vacation_API.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly UsersController _controller;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly VacationDbContext _context;
        private readonly List<User> _users;

        public UsersControllerTests()
        {
            // Create test users
            _users = new List<User>
            {
                new User { 
                    Id = "user1", 
                    UserName = "user1@example.com", 
                    Email = "user1@example.com",
                    FirstName = "Test",
                    LastName = "User",
                    Address = "123 Test St",
                    PhoneNumber = "555-1234"
                },
                new User { 
                    Id = "user2", 
                    UserName = "admin@example.com", 
                    Email = "admin@example.com",
                    FirstName = "Admin",
                    LastName = "User",
                    Address = "456 Admin Rd",
                    PhoneNumber = "555-5678",
                    IsAdmin = true
                }
            };

            // Create in-memory database
            var options = new DbContextOptionsBuilder<VacationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestUsersDb")
                .Options;

            _context = new VacationDbContext(options);
            _context.Database.EnsureDeleted();
            
            // Add a booking for the first user
            _context.Vacations.Add(new Vacation {
                VacationID = 1,
                Name = "Test Vacation",
                Description = "Description",
                PricePerNight = 100m,
                DestinationID = 1,
                AccommodationID = 1,
                AvailableRooms = 5
            });
            
            _context.Bookings.Add(new Booking {
                BookingID = 1,
                UserID = "user1",
                VacationID = 1,
                CheckInDate = System.DateTime.Now.AddDays(10),
                NumberOfNights = 3,
                NumberOfGuests = 2,
                SpecialRequests = "None",
                TotalPrice = 300m
            });
            
            _context.SaveChanges();

            // Set up UserManager mock
            var userStoreMock = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            // Mock basic UserManager methods
            _mockUserManager.Setup(um => um.Users).Returns(_users.AsQueryable());
            
            _mockUserManager
                .Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _users.FirstOrDefault(u => u.Id == id));
                
            _mockUserManager
                .Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
                
            _mockUserManager
                .Setup(um => um.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);
                
            _mockUserManager
                .Setup(um => um.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            _controller = new UsersController(_mockUserManager.Object, _context);
        }

        // Happy path tests
        [Fact]
        public void GetUsers_ReturnsAllUsers()
        {
            // Act
            var result = _controller.GetUsers();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<User>>>(result);
            var users = Assert.IsAssignableFrom<IEnumerable<User>>(actionResult.Value);
            Assert.Equal(2, users.Count());
        }

        [Fact]
        public async Task GetUser_WithValidId_ReturnsUser()
        {
            // Act
            var result = await _controller.GetUser("user1");

            // Assert
            var actionResult = Assert.IsType<ActionResult<User>>(result);
            var user = Assert.IsType<User>(actionResult.Value);
            Assert.Equal("user1@example.com", user.Email);
        }

        [Fact]
        public async Task GetUserBookings_WithValidId_ReturnsUserBookings()
        {
            // Act
            var result = await _controller.GetUserBookings("user1");

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Booking>>>(result);
            var bookings = Assert.IsAssignableFrom<IEnumerable<Booking>>(actionResult.Value);
            Assert.Single(bookings);
        }

        [Fact]
        public async Task PostUser_WithValidModel_CreatesUser()
        {
            // Arrange
            var newUser = new User
            {
                Email = "new@example.com",
                FirstName = "New",
                LastName = "User",
                Address = "789 New St",
                Password = "Password123!"
            };

            // Act
            var result = await _controller.PostUser(newUser);

            // Assert
            var actionResult = Assert.IsType<ActionResult<User>>(result);
            Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        }

        // Error path tests
        [Fact]
        public async Task GetUser_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetUser("nonexistent-id");

            // Assert
            var actionResult = Assert.IsType<ActionResult<User>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task PostUser_WithoutPassword_ReturnsBadRequest()
        {
            // Arrange
            var invalidUser = new User
            {
                Email = "invalid@example.com",
                FirstName = "Invalid",
                LastName = "User",
                Password = null // Missing password
            };

            // Act
            var result = await _controller.PostUser(invalidUser);

            // Assert
            var actionResult = Assert.IsType<ActionResult<User>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task DeleteUser_WithBookings_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.DeleteUser("user1"); // user with bookings

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}

using VacationBooking.Models;
using Xunit;

namespace Vacation_Frontend.Tests.Models
{
    public class UserTests
    {
        [Fact]
        public void CanSetAndGetProperties()
        {
            // Arrange
            var user = new User();
            
            // Act
            user.Id = "user123";
            user.Email = "test@example.com";
            user.UserName = "test@example.com";
            user.FirstName = "John";
            user.LastName = "Doe";
            user.Address = "123 Main St";
            user.Password = "password123";
            user.IsAdmin = true;

            // Assert
            Assert.Equal("user123", user.Id);
            Assert.Equal("test@example.com", user.Email);
            Assert.Equal("test@example.com", user.UserName);
            Assert.Equal("John", user.FirstName);
            Assert.Equal("Doe", user.LastName);
            Assert.Equal("123 Main St", user.Address);
            Assert.Equal("password123", user.Password);
            Assert.True(user.IsAdmin);
        }

        [Fact]
        public void BookingsCollectionIsInitialized()
        {
            // Arrange & Act
            var user = new User();
            
            // Assert
            Assert.NotNull(user.Bookings);
            Assert.Empty(user.Bookings);
        }
    }
} 
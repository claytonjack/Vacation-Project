using VacationBooking.Models;
using Xunit;

namespace Vacation_Frontend.Tests.Models
{
    public class LoginTests
    {
        [Fact]
        public void CanSetAndGetProperties()
        {
            // Arrange
            var login = new Login();
            
            // Act
            login.Email = "test@example.com";
            login.Password = "password123";
            login.Remember = true;
            login.ReturnUrl = "/home";

            // Assert
            Assert.Equal("test@example.com", login.Email);
            Assert.Equal("password123", login.Password);
            Assert.True(login.Remember);
            Assert.Equal("/home", login.ReturnUrl);
        }
    }
} 
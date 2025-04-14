using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using VacationBooking.Models;
using Xunit;

namespace Vacation_API.Tests.Models
{
    public class UserTests
    {
        [Fact]
        public void User_PropertiesGetAndSet()
        {
            var user = new User
            {
                Id = "user-123",
                UserName = "test@example.com",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Address = "123 Main St",
                Password = "Password123!",
                IsAdmin = true
            };

            Assert.Equal("user-123", user.Id);
            Assert.Equal("test@example.com", user.UserName);
            Assert.Equal("test@example.com", user.Email);
            Assert.Equal("John", user.FirstName);
            Assert.Equal("Doe", user.LastName);
            Assert.Equal("123 Main St", user.Address);
            Assert.Equal("Password123!", user.Password);
            Assert.True(user.IsAdmin);
        }

        [Fact]
        public void User_RequiredFieldsValidation()
        {
            var user = new User();
            
            var validationContext = new ValidationContext(user);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(user, validationContext, validationResults, true);
            
            var propertyNames = new List<string>();
            foreach (var validationResult in validationResults)
            {
                propertyNames.AddRange(validationResult.MemberNames);
            }
            
            Assert.Contains("FirstName", propertyNames);
            Assert.Contains("LastName", propertyNames);
            Assert.Contains("Password", propertyNames);
        }

        [Fact]
        public void User_DefaultsAreSet()
        {
            var user = new User();
            
            Assert.False(user.IsAdmin);
            Assert.NotNull(user.Bookings);
            Assert.Empty(user.Bookings);
        }
    }
} 
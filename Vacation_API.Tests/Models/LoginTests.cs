using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using VacationBooking.Models;
using Xunit;

namespace Vacation_API.Tests.Models
{
    public class LoginTests
    {
        [Fact]
        public void Login_PropertiesGetAndSet()
        {
            var login = new Login
            {
                Email = "test@example.com",
                Password = "Password123",
                Remember = true,
                ReturnUrl = "/home"
            };

            Assert.Equal("test@example.com", login.Email);
            Assert.Equal("Password123", login.Password);
            Assert.True(login.Remember);
            Assert.Equal("/home", login.ReturnUrl);
        }

        [Fact]
        public void Login_RequiredFieldsValidation()
        {
            var login = new Login();
            
            var validationContext = new ValidationContext(login);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(login, validationContext, validationResults, true);
            
            var propertyNames = new List<string>();
            foreach (var validationResult in validationResults)
            {
                propertyNames.AddRange(validationResult.MemberNames);
            }
            
            Assert.Contains("Email", propertyNames);
            Assert.Contains("Password", propertyNames);
        }

        [Fact]
        public void Login_EmailValidation()
        {
            var login = new Login
            {
                Email = "not-an-email",
                Password = "Password123"
            };
            
            var validationContext = new ValidationContext(login);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(login, validationContext, validationResults, true);
            
            var propertyNames = new List<string>();
            foreach (var validationResult in validationResults)
            {
                propertyNames.AddRange(validationResult.MemberNames);
            }
            
            Assert.Contains("Email", propertyNames);
        }
    }
} 
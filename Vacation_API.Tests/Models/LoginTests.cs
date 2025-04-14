using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using VacationBooking.Models;
using Xunit;

namespace Vacation_API.Tests.Models
{
    public class LoginTests
    {
        //test for login
        [Fact]
        public void Login_PropertiesGetAndSet()
        {
            //act
            var login = new Login
            {
                Email = "test@example.com",
                Password = "Password123",
                Remember = true,
                ReturnUrl = "/home"
            };
            //assert
            Assert.Equal("test@example.com", login.Email);
            Assert.Equal("Password123", login.Password);
            Assert.True(login.Remember);
            Assert.Equal("/home", login.ReturnUrl);
        }

        // login validation test
        [Fact]
        public void Login_RequiredFieldsValidation()
        {
            //act
            var login = new Login();
            
            var validationContext = new ValidationContext(login);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(login, validationContext, validationResults, true);
            
            var propertyNames = new List<string>();
            foreach (var validationResult in validationResults)
            {
                propertyNames.AddRange(validationResult.MemberNames);
            }
            //assert
            Assert.Contains("Email", propertyNames);
            Assert.Contains("Password", propertyNames);
        }

        //email validation test
        [Fact]
        public void Login_EmailValidation()
        {
            //act
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
            //assert
            Assert.Contains("Email", propertyNames);
        }
    }
} 
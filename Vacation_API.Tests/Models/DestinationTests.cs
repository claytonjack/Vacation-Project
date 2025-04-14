using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using VacationBooking.Models;
using Xunit;

namespace Vacation_API.Tests.Models
{
    public class DestinationTests
    {
        [Fact]
        public void Destination_PropertiesGetAndSet()
        {
            //act
            var destination = new Destination
            {
                DestinationID = 1,
                City = "Paris",
                Country = "France",
                ImageUrl = "/images/paris.jpg"
            };
            //asert
            Assert.Equal(1, destination.DestinationID);
            Assert.Equal("Paris", destination.City);
            Assert.Equal("France", destination.Country);
            Assert.Equal("/images/paris.jpg", destination.ImageUrl);
        }

        [Fact]
        public void Destination_RequiredFieldsValidation()
        {
            //act
            var destination = new Destination();
            
            var validationContext = new ValidationContext(destination);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(destination, validationContext, validationResults, true);
            
            var propertyNames = new List<string>();
            foreach (var validationResult in validationResults)
            {
                propertyNames.AddRange(validationResult.MemberNames);
            }
            //asert
            Assert.Contains("City", propertyNames);
            Assert.Contains("Country", propertyNames);
        }

        [Fact]
        public void Destination_VacationsCollectionInitialized()
        {
            //act
            var destination = new Destination();
            //asert
            Assert.NotNull(destination.Vacations);
            Assert.Empty(destination.Vacations);
        }
    }
} 
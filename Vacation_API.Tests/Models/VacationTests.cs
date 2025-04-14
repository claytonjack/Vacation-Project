using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using VacationBooking.Models;
using Xunit;

namespace Vacation_API.Tests.Models
{
    public class VacationTests
    {
        [Fact]
        public void Vacation_PropertiesGetAndSet()
        {
            var vacation = new Vacation
            {
                VacationID = 1,
                Name = "Paris Getaway",
                Description = "Romantic weekend in Paris",
                PricePerNight = 199.99m,
                AllInclusive = true,
                AvailableRooms = 5,
                ImageUrl = "/images/paris.jpg",
                DestinationID = 1,
                AccommodationID = 1
            };

            Assert.Equal(1, vacation.VacationID);
            Assert.Equal("Paris Getaway", vacation.Name);
            Assert.Equal("Romantic weekend in Paris", vacation.Description);
            Assert.Equal(199.99m, vacation.PricePerNight);
            Assert.True(vacation.AllInclusive);
            Assert.Equal(5, vacation.AvailableRooms);
            Assert.Equal("/images/paris.jpg", vacation.ImageUrl);
            Assert.Equal(1, vacation.DestinationID);
            Assert.Equal(1, vacation.AccommodationID);
        }

        [Fact]
        public void Vacation_RequiredFieldsValidation()
        {
            var vacation = new Vacation();
            
            var validationContext = new ValidationContext(vacation);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(vacation, validationContext, validationResults, true);
            
            var propertyNames = new List<string>();
            foreach (var validationResult in validationResults)
            {
                propertyNames.AddRange(validationResult.MemberNames);
            }
            
            Assert.Contains("Name", propertyNames);
            Assert.Contains("Description", propertyNames);
        }

        [Fact]
        public void Vacation_DefaultStringProperties()
        {
            var vacation = new Vacation();
            
            Assert.Equal(string.Empty, vacation.Name);
            Assert.Equal(string.Empty, vacation.Description);
        }

        [Fact]
        public void Vacation_BookingsCollectionInitialized()
        {
            var vacation = new Vacation();
            
            Assert.NotNull(vacation.Bookings);
            Assert.Empty(vacation.Bookings);
        }
    }
} 
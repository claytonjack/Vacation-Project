using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using VacationBooking.Models;
using Xunit;

namespace Vacation_API.Tests.Models
{
    public class AccommodationTests
    {
        [Fact]
        public void Accommodation_PropertiesGetAndSet()
        {
            //Act
            var accommodation = new Accommodation
            {
                AccommodationID = 1,
                HotelName = "Test Hotel",
                Address = "123 Test St",
                RoomType = "Deluxe",
                ImageUrl = "/images/test.jpg"
            };
            //Assert
            Assert.Equal(1, accommodation.AccommodationID);
            Assert.Equal("Test Hotel", accommodation.HotelName);
            Assert.Equal("123 Test St", accommodation.Address);
            Assert.Equal("Deluxe", accommodation.RoomType);
            Assert.Equal("/images/test.jpg", accommodation.ImageUrl);
        }

        [Fact]
        public void Accommodation_RequiredFieldsValidation()
        {
            //Act
            var accommodation = new Accommodation();
            
            var validationContext = new ValidationContext(accommodation);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(accommodation, validationContext, validationResults, true);
            
            var propertyNames = new List<string>();
            foreach (var validationResult in validationResults)
            {
                propertyNames.AddRange(validationResult.MemberNames);
            }
            //Assert
            Assert.Contains("HotelName", propertyNames);
            Assert.Contains("Address", propertyNames);
            Assert.Contains("RoomType", propertyNames);
        }

        [Fact]
        public void Accommodation_VacationsCollectionInitialized()
        {
            //Act
            var accommodation = new Accommodation();
            //Assert
            Assert.NotNull(accommodation.Vacations);
            Assert.Empty(accommodation.Vacations);
        }
    }
} 
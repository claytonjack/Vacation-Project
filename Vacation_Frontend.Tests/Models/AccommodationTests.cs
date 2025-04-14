using VacationBooking.Models;
using Xunit;

namespace Vacation_Frontend.Tests.Models
{
    public class AccommodationTests
    {
        [Fact]
        public void CanSetAndGetProperties()
        {
            // Arrange
            var accommodation = new Accommodation();
            
            // Act
            accommodation.AccommodationID = 1;
            accommodation.HotelName = "Test Hotel";
            accommodation.Address = "123 Test St";
            accommodation.RoomType = "Deluxe";
            accommodation.ImageUrl = "/images/test.jpg";

            // Assert
            Assert.Equal(1, accommodation.AccommodationID);
            Assert.Equal("Test Hotel", accommodation.HotelName);
            Assert.Equal("123 Test St", accommodation.Address);
            Assert.Equal("Deluxe", accommodation.RoomType);
            Assert.Equal("/images/test.jpg", accommodation.ImageUrl);
        }

        [Fact]
        public void VacationsCollectionIsInitialized()
        {
            // Arrange & Act
            var accommodation = new Accommodation();
            
            // Assert
            Assert.NotNull(accommodation.Vacations);
            Assert.Empty(accommodation.Vacations);
        }
    }
} 
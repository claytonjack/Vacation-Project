using VacationBooking.Models;
using Xunit;

namespace Vacation_Frontend.Tests.Models
{
    public class VacationTests
    {
        [Fact]
        public void CanSetAndGetProperties()
        {
            // Arrange
            var vacation = new Vacation();
            
            // Act
            vacation.VacationID = 1;
            vacation.Name = "Paris Getaway";
            vacation.Description = "Romantic weekend in Paris";
            vacation.PricePerNight = 199.99m;
            vacation.AllInclusive = true;
            vacation.AvailableRooms = 5;
            vacation.ImageUrl = "/images/paris.jpg";
            vacation.DestinationID = 2;
            vacation.AccommodationID = 3;

            // Assert
            Assert.Equal(1, vacation.VacationID);
            Assert.Equal("Paris Getaway", vacation.Name);
            Assert.Equal("Romantic weekend in Paris", vacation.Description);
            Assert.Equal(199.99m, vacation.PricePerNight);
            Assert.True(vacation.AllInclusive);
            Assert.Equal(5, vacation.AvailableRooms);
            Assert.Equal("/images/paris.jpg", vacation.ImageUrl);
            Assert.Equal(2, vacation.DestinationID);
            Assert.Equal(3, vacation.AccommodationID);
        }

        [Fact]
        public void DefaultsForStringProperties()
        {
            // Arrange
            var vacation = new Vacation();
            
            // Act & Assert
            Assert.Equal(string.Empty, vacation.Name);
            Assert.Equal(string.Empty, vacation.Description);
        }

        [Fact]
        public void BookingsCollectionIsInitialized()
        {
            // Arrange & Act
            var vacation = new Vacation();
            
            // Assert
            Assert.NotNull(vacation.Bookings);
            Assert.Empty(vacation.Bookings);
        }
    }
} 
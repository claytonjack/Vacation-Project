using VacationBooking.Models;
using Xunit;

namespace Vacation_Frontend.Tests.Models
{
    public class DestinationTests
    {
        [Fact]
        public void CanSetAndGetProperties()
        {
            // Arrange
            var destination = new Destination();
            
            // Act
            destination.DestinationID = 1;
            destination.City = "Paris";
            destination.Country = "France";
            destination.ImageUrl = "/images/paris.jpg";

            // Assert
            Assert.Equal(1, destination.DestinationID);
            Assert.Equal("Paris", destination.City);
            Assert.Equal("France", destination.Country);
            Assert.Equal("/images/paris.jpg", destination.ImageUrl);
        }

        [Fact]
        public void VacationsCollectionIsInitialized()
        {
            // Arrange & Act
            var destination = new Destination();
            
            // Assert
            Assert.NotNull(destination.Vacations);
            Assert.Empty(destination.Vacations);
        }
    }
} 
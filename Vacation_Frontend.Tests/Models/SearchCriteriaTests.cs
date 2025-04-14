using VacationBooking.Models;
using Xunit;

namespace Vacation_Frontend.Tests.Models
{
    public class SearchCriteriaTests
    {
        //search test
        [Fact]
        public void CanSetAndGetProperties()
        {
            // Arrange
            var criteria = new SearchCriteria();
            
            // Act
            criteria.City = "London";
            criteria.Country = "England";
            criteria.MaxPricePerNight = 299.99m;
            criteria.RoomType = "Hotel";
            criteria.AllInclusive = true;

            // Assert
            Assert.Equal("London", criteria.City);
            Assert.Equal("England", criteria.Country);
            Assert.Equal(299.99m, criteria.MaxPricePerNight);
            Assert.Equal("Hotel", criteria.RoomType);
            Assert.True(criteria.AllInclusive);
        }
    }
} 
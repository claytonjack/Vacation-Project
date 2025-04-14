using VacationBooking.Models;
using Xunit;

namespace Vacation_API.Tests.Models
{
    public class SearchCriteriaTests
    {
        [Fact]
        public void SearchCriteria_PropertiesGetAndSet()
        {
            var criteria = new SearchCriteria
            {
                City = "Paris",
                Country = "France",
                MaxPricePerNight = 200.0m,
                RoomType = "Deluxe",
                AllInclusive = true
            };

            Assert.Equal("Paris", criteria.City);
            Assert.Equal("France", criteria.Country);
            Assert.Equal(200.0m, criteria.MaxPricePerNight);
            Assert.Equal("Deluxe", criteria.RoomType);
            Assert.True(criteria.AllInclusive);
        }

        [Fact]
        public void SearchCriteria_PropertiesAreOptional()
        {
            var criteria = new SearchCriteria();
            
            Assert.NotNull(criteria);
            Assert.Null(criteria.City);
            Assert.Null(criteria.Country);
            Assert.Equal(0m, criteria.MaxPricePerNight);
            Assert.Null(criteria.RoomType);
            Assert.Null(criteria.AllInclusive);
        }
    }
} 
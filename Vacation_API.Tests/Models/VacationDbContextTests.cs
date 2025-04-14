using Microsoft.EntityFrameworkCore;
using VacationBooking.Data;
using VacationBooking.Models;
using Xunit;

namespace Vacation_API.Tests.Models
{
    public class VacationDbContextTests
    {
        [Fact]
        public void DbContext_SetsAreCreated()
        {
            var options = new DbContextOptionsBuilder<VacationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDbContext")
                .Options;
                
            using var context = new VacationDbContext(options);
            
            Assert.NotNull(context.Vacations);
            Assert.NotNull(context.Destinations);
            Assert.NotNull(context.Accommodations);
            Assert.NotNull(context.Bookings);
            Assert.NotNull(context.Users);
        }

        [Fact]
        public void DbContext_RelationshipsAreConfigured()
        {
            var options = new DbContextOptionsBuilder<VacationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestRelationships")
                .Options;
                
            using (var context = new VacationDbContext(options))
            {
                var destination = new Destination { City = "Paris", Country = "France" };
                var accommodation = new Accommodation { HotelName = "Test Hotel", Address = "123 Test St", RoomType = "Standard" };
                
                context.Destinations.Add(destination);
                context.Accommodations.Add(accommodation);
                context.SaveChanges();
                
                var vacation = new Vacation
                {
                    Name = "Test Vacation",
                    Description = "Test Description",
                    PricePerNight = 100m,
                    AvailableRooms = 5,
                    DestinationID = destination.DestinationID,
                    AccommodationID = accommodation.AccommodationID
                };
                
                context.Vacations.Add(vacation);
                context.SaveChanges();
            }
            
            using (var context = new VacationDbContext(options))
            {
                var vacation = context.Vacations
                    .Include(v => v.Destination)
                    .Include(v => v.Accommodation)
                    .FirstOrDefault();
                    
                Assert.NotNull(vacation);
                Assert.NotNull(vacation.Destination);
                Assert.NotNull(vacation.Accommodation);
                Assert.Equal("Paris", vacation.Destination.City);
                Assert.Equal("Test Hotel", vacation.Accommodation.HotelName);
            }
        }
    }
} 
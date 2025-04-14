using System;
using VacationBooking.Models;
using Xunit;

namespace Vacation_Frontend.Tests.Models
{
    public class BookingTests
    {
        [Fact]
        public void CanSetAndGetProperties()
        {
            // Arrange
            var booking = new Booking();
            var now = DateTime.Now;
            var checkIn = DateTime.Now.AddDays(5);
            
            // Act
            booking.BookingID = 1;
            booking.UserID = "user123";
            booking.VacationID = 2;
            booking.CheckInDate = checkIn;
            booking.NumberOfNights = 3;
            booking.NumberOfGuests = 2;
            booking.TotalPrice = 299.97m;
            booking.BookingDate = now;
            booking.SpecialRequests = "Late checkout requested";

            // Assert
            Assert.Equal(1, booking.BookingID);
            Assert.Equal("user123", booking.UserID);
            Assert.Equal(2, booking.VacationID);
            Assert.Equal(checkIn, booking.CheckInDate);
            Assert.Equal(3, booking.NumberOfNights);
            Assert.Equal(2, booking.NumberOfGuests);
            Assert.Equal(299.97m, booking.TotalPrice);
            Assert.Equal(now, booking.BookingDate);
            Assert.Equal("Late checkout requested", booking.SpecialRequests);
        }

        //booking with current time test
        [Fact]
        public void BookingDateDefaultsToCurrentTime()
        {
            // Arrange & Act
            var booking = new Booking();
            
            // Assert - Check that BookingDate is set to approximately current time
            var timeDifference = (DateTime.Now - booking.BookingDate).TotalSeconds;
            Assert.True(timeDifference < 5, "BookingDate should default to approximately current time");
        }
    }
} 
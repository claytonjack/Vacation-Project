using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using VacationBooking.Models;
using Xunit;

namespace Vacation_API.Tests.Models
{
    public class BookingTests
    {
        [Fact]
        public void Booking_PropertiesGetAndSet()
        {
            //Act
            var now = DateTime.Now;
            var checkIn = DateTime.Now.AddDays(7);
            
            var booking = new Booking
            {
                BookingID = 1,
                UserID = "user1",
                VacationID = 1,
                CheckInDate = checkIn,
                NumberOfNights = 3,
                NumberOfGuests = 2,
                TotalPrice = 300m,
                BookingDate = now,
                SpecialRequests = "Late check-in"
            };
            //Assert
            Assert.Equal(1, booking.BookingID);
            Assert.Equal("user1", booking.UserID);
            Assert.Equal(1, booking.VacationID);
            Assert.Equal(checkIn, booking.CheckInDate);
            Assert.Equal(3, booking.NumberOfNights);
            Assert.Equal(2, booking.NumberOfGuests);
            Assert.Equal(300m, booking.TotalPrice);
            Assert.Equal(now, booking.BookingDate);
            Assert.Equal("Late check-in", booking.SpecialRequests);
        }

        [Fact]
        public void Booking_RequiredFieldsValidation()
        {
            //act
            var booking = new Booking();
            
            var validationContext = new ValidationContext(booking);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(booking, validationContext, validationResults, true);
            
            var propertyNames = new List<string>();
            foreach (var validationResult in validationResults)
            {
                propertyNames.AddRange(validationResult.MemberNames);
            }
            //Assert
            Assert.Contains("UserID", propertyNames);
            Assert.Contains("NumberOfNights", propertyNames);
            Assert.Contains("NumberOfGuests", propertyNames);
        }

        [Fact]
        public void Booking_RangeValidation()
        {
            //act
            var booking = new Booking
            {
                UserID = "user1",
                VacationID = 1,
                CheckInDate = DateTime.Now.AddDays(5),
                NumberOfNights = 35,
                NumberOfGuests = 15
            };
            
            var validationContext = new ValidationContext(booking);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(booking, validationContext, validationResults, true);
            
            var propertyNames = new List<string>();
            foreach (var validationResult in validationResults)
            {
                propertyNames.AddRange(validationResult.MemberNames);
            }
            //Assert
            Assert.Contains("NumberOfNights", propertyNames);
            Assert.Contains("NumberOfGuests", propertyNames);
        }

        [Fact]
        public void Booking_DefaultsAreSet()
        {
            //act
            var booking = new Booking();
            
            var timeDifference = Math.Abs((DateTime.Now - booking.BookingDate).TotalSeconds);
            //Assert
            Assert.True(timeDifference < 10);
        }
    }
} 
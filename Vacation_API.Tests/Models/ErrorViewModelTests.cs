using VacationBooking.Models;
using Xunit;

namespace Vacation_API.Tests.Models
{
    public class ErrorViewModelTests
    {
        [Fact]
        public void ErrorViewModel_PropertiesGetAndSet()
        {
            //act
            var model = new ErrorViewModel
            {
                RequestId = "TEST-ID-123"
            };
            //assert
            Assert.Equal("TEST-ID-123", model.RequestId);
        }

        [Fact]
        public void ShowRequestId_ReturnsTrueWhenRequestIdIsPresent()
        {
            //act
            var model = new ErrorViewModel
            {
                RequestId = "TEST-ID-123"
            };
            //assert
            Assert.True(model.ShowRequestId);
        }

        [Fact]
        public void ShowRequestId_ReturnsFalseWhenRequestIdIsEmpty()
        {
            //act
            var model = new ErrorViewModel
            {
                RequestId = ""
            };
            //assert
            Assert.False(model.ShowRequestId);
        }

        [Fact]
        public void ShowRequestId_ReturnsFalseWhenRequestIdIsNull()
        {
            //act
            var model = new ErrorViewModel
            {
                RequestId = null
            };
            //assert
            Assert.False(model.ShowRequestId);
        }
    }
} 
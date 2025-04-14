using VacationBooking.Models;
using Xunit;

namespace Vacation_API.Tests.Models
{
    public class ErrorViewModelTests
    {
        [Fact]
        public void ErrorViewModel_PropertiesGetAndSet()
        {
            var model = new ErrorViewModel
            {
                RequestId = "TEST-ID-123"
            };

            Assert.Equal("TEST-ID-123", model.RequestId);
        }

        [Fact]
        public void ShowRequestId_ReturnsTrueWhenRequestIdIsPresent()
        {
            var model = new ErrorViewModel
            {
                RequestId = "TEST-ID-123"
            };

            Assert.True(model.ShowRequestId);
        }

        [Fact]
        public void ShowRequestId_ReturnsFalseWhenRequestIdIsEmpty()
        {
            var model = new ErrorViewModel
            {
                RequestId = ""
            };

            Assert.False(model.ShowRequestId);
        }

        [Fact]
        public void ShowRequestId_ReturnsFalseWhenRequestIdIsNull()
        {
            var model = new ErrorViewModel
            {
                RequestId = null
            };

            Assert.False(model.ShowRequestId);
        }
    }
} 
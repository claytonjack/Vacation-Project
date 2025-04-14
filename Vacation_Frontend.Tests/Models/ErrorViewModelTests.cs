using VacationBooking.Models;
using Xunit;

namespace Vacation_Frontend.Tests.Models
{
    public class ErrorViewModelTests
    {
        [Fact]
        public void CanSetAndGetRequestId()
        {
            // Arrange
            var errorViewModel = new ErrorViewModel();
            
            // Act
            errorViewModel.RequestId = "TEST-123";

            // Assert
            Assert.Equal("TEST-123", errorViewModel.RequestId);
        }

        //request id test
        [Fact]
        public void ShowRequestIdReturnsTrueWhenRequestIdIsSet()
        {
            // Arrange
            var errorViewModel = new ErrorViewModel
            {
                RequestId = "TEST-123"
            };
            
            // Act & Assert
            Assert.True(errorViewModel.ShowRequestId);
        }

        //id empty test
        [Fact]
        public void ShowRequestIdReturnsFalseWhenRequestIdIsEmpty()
        {
            // Arrange
            var errorViewModel = new ErrorViewModel
            {
                RequestId = ""
            };
            
            // Act & Assert
            Assert.False(errorViewModel.ShowRequestId);
        }

        //id null test
        [Fact]
        public void ShowRequestIdReturnsFalseWhenRequestIdIsNull()
        {
            // Arrange
            var errorViewModel = new ErrorViewModel
            {
                RequestId = null
            };
            
            // Act & Assert
            Assert.False(errorViewModel.ShowRequestId);
        }
    }
} 
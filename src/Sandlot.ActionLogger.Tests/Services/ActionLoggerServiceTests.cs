using Microsoft.Extensions.Logging;
using Moq;
using Sandlot.ActionLogger.Services;

namespace Sandlot.ActionLogger.Tests.Services
{
    public class ActionLoggerServiceTests
    {
        private ActionLoggerService CreateSUT(Mock<ILogger<ActionLoggerService>>? loggerMock = null)
        {
            return new ActionLoggerService(loggerMock?.Object ?? new Mock<ILogger<ActionLoggerService>>().Object);
        }

        [Fact]
        public void BeginStep_ShouldNotLog_WhenLogToLoggerIsFalse()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ActionLoggerService>>();
            var sut = CreateSUT(loggerMock);

            // Act
            using (sut.BeginStep("Test Step", logToLogger: false))
            {
                // No-op
            }

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("BEGIN") || v.ToString()!.Contains("DONE")),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Never);
        }

        [Fact]
        public void BeginStep_ShouldLogStartAndEnd_WhenLogToLoggerIsTrue()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ActionLoggerService>>();
            var sut = CreateSUT(loggerMock);

            // Act
            using (sut.BeginStep("Test Step", logToLogger: true))
            {
                // Simulate some work
            }

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("BEGIN")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("DONE") || v.ToString()!.Contains("SLOW")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void Success_ShouldLog_WhenLogToLoggerIsTrue()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ActionLoggerService>>();
            var sut = CreateSUT(loggerMock);

            // Act
            sut.Success("Completed", logToLogger: true);

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Completed")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void ErrorAndMaybeThrow_ShouldLogError_AndReturnValidationResult_WhenThrowIsFalse()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ActionLoggerService>>();
            var sut = CreateSUT(loggerMock);
            var message = "This is a validation failure.";

            // Act
            var result = sut.ErrorAndMaybeThrow(message, throwException: false);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(message, result.Message);
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains(message)),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void ErrorAndMaybeThrow_ShouldThrowGenericException_WhenThrowIsTrue_AndNoFactoryProvided()
        {
            // Arrange
            var sut = CreateSUT();
            var message = "Must throw an exception.";

            // Act & Assert
            var ex = Assert.Throws<Exception>(() =>
                sut.ErrorAndMaybeThrow(message, throwException: true)
            );

            Assert.Equal(message, ex.Message);
        }

        [Fact]
        public void ErrorAndMaybeThrow_ShouldThrowCustomException_WhenFactoryIsProvided()
        {
            // Arrange
            var sut = CreateSUT();
            var message = "Custom failure!";
            Func<string, Exception> factory = msg => new InvalidOperationException(msg);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
                sut.ErrorAndMaybeThrow(message, throwException: true, exceptionFactory: factory)
            );

            Assert.Equal(message, ex.Message);
        }

    }
}
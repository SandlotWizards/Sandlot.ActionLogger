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
    }
}
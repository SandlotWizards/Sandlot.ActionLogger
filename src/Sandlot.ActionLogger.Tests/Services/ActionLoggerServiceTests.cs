using Microsoft.Extensions.Logging;
using Moq;
using Sandlot.ActionLogger.Services;
using Xunit;

namespace Sandlot.ActionLogger.Tests.Services
{
    public class ActionLoggerServiceTests
    {
        [Fact]
        public void Info_ShouldLogMessage()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ActionLoggerService>>();
            var service = new ActionLoggerService(loggerMock.Object);

            // Act
            var result = service.Info("Test Info");

            // Assert
            Assert.Equal("Test Info", result);
        }

        [Fact]
        public void Success_ShouldReturnDefaultSuccessMessage()
        {
            var loggerMock = new Mock<ILogger<ActionLoggerService>>();
            var service = new ActionLoggerService(loggerMock.Object);

            var result = service.Success();

            Assert.Equal("✔ Done", result);
        }

        [Fact]
        public void BeginStep_And_Dispose_ShouldNotThrow()
        {
            var loggerMock = new Mock<ILogger<ActionLoggerService>>();
            var service = new ActionLoggerService(loggerMock.Object);

            using var step = service.BeginStep("Test Step");
            Assert.NotNull(step);
        }

        [Fact]
        public void ErrorResult_ShouldReturnFailedValidationResult()
        {
            var loggerMock = new Mock<ILogger<ActionLoggerService>>();
            var service = new ActionLoggerService(loggerMock.Object);

            var result = service.ErrorResult("something went wrong");

            Assert.False(result.IsValid);
            Assert.Equal("something went wrong", result.Message);
        }

        [Fact]
        public void SuccessResult_ShouldReturnValidValidationResult()
        {
            var loggerMock = new Mock<ILogger<ActionLoggerService>>();
            var service = new ActionLoggerService(loggerMock.Object);

            var result = service.SuccessResult();

            Assert.True(result.IsValid);
        }
    }
}
using Microsoft.Extensions.Logging;
using Moq;
using Sandlot.ActionLogger.Services;

namespace Sandlot.ActionLogger.Tests.Services
{
    public class ActionLoggerServiceTests
    {
        private readonly Mock<ILogger<ActionLoggerService>> _mockLogger;
        private readonly ActionLoggerService _loggerService;

        public ActionLoggerServiceTests()
        {
            _mockLogger = new Mock<ILogger<ActionLoggerService>>();
            _loggerService = new ActionLoggerService(_mockLogger.Object);
        }

        [Fact]
        public void Trace_ShouldLogMessage()
        {
            var message = _loggerService.Trace("trace test");
            Assert.Equal("trace test", message);
        }

        [Fact]
        public void Debug_ShouldLogMessage()
        {
            var message = _loggerService.Debug("debug test");
            Assert.Equal("debug test", message);
        }

        [Fact]
        public void Info_ShouldLogMessage()
        {
            var message = _loggerService.Info("info test");
            Assert.Equal("info test", message);
        }

        [Fact]
        public void Warning_ShouldLogMessage()
        {
            var message = _loggerService.Warning("warning test");
            Assert.Equal("warning test", message);
        }

        [Fact]
        public void Error_ShouldLogMessage()
        {
            var message = _loggerService.Error("error test");
            Assert.Equal("error test", message);
        }

        [Fact]
        public void Critical_ShouldLogMessage()
        {
            var message = _loggerService.Critical("critical test");
            Assert.Equal("critical test", message);
        }

        [Fact]
        public void Success_ShouldLogMessage()
        {
            var message = _loggerService.Success("✔ success test");
            Assert.Equal("✔ success test", message);
        }

        [Fact]
        public void Message_ShouldLogMessageWithoutSymbol()
        {
            var message = _loggerService.Message("plain message");
            Assert.Equal("plain message", message);
        }

        [Fact]
        public void BeginStep_ShouldDisposeWithoutError()
        {
            using var step = _loggerService.BeginStep("Sample Step");
            Assert.NotNull(step);
        }

        [Fact]
        public void Info_WithException_ShouldThrow()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _loggerService.Info(
                    "throwing info",
                    throwException: true,
                    exceptionFactory: msg => new InvalidOperationException(msg)));
        }

        [Fact]
        public void Error_WithDefaultException_ShouldThrow()
        {
            Assert.Throws<Exception>(() =>
                _loggerService.Error("throwing error", throwException: true));
        }

        [Fact]
        public void Message_WithException_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _loggerService.Message("throw here", throwException: true, exceptionFactory: _ => new ArgumentNullException()));
        }
    }
}
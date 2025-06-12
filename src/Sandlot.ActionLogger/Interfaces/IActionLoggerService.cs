using System;
using Sandlot.ActionLogger.Models;

namespace Sandlot.ActionLogger.Interfaces
{
    /// <summary>
    /// Provides structured and level-based logging with support for console output and validation result tracking.
    /// </summary>
    public interface IActionLoggerService
    {
        /// <summary>
        /// Begins a new step in the logging lifecycle.
        /// </summary>
        IDisposable BeginStep(string title, TimeSpan? threshold = null, bool logToLogger = false);

        string Trace(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true);
        string Debug(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true);
        string Info(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true);
        string Warning(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true);
        string Error(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true);
        string Critical(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true);
        string Success(string message = "✔ Done", bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = false);
        string Message(string message, ConsoleColor color = ConsoleColor.Gray, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = false);

        void PrintHeader(ConsoleColor color = ConsoleColor.Gray);
        void PrintTrailer(ConsoleColor color = ConsoleColor.Gray);

        ValidationResult TraceResult(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true);
        ValidationResult DebugResult(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true);
        ValidationResult InfoResult(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true);
        ValidationResult WarningResult(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true);
        ValidationResult ErrorResult(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true);
        ValidationResult CriticalResult(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true);
        ValidationResult SuccessResult(string message = "✔ Done", bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = false);
        ValidationResult MessageResult(string message, ConsoleColor color = ConsoleColor.Gray, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = false);
    }
}
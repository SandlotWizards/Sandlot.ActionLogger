using System;
using Sandlot.ActionLogger.Models;

namespace Sandlot.ActionLogger.Interfaces
{
    /// <summary>
    /// Provides structured step tracking with hierarchical step numbers,
    /// visual output to the console, and optional logging to ILogger.
    /// </summary>
    public interface IActionLoggerService
    {
        IDisposable BeginStep(string title, TimeSpan? threshold = null, bool logToLogger = false);
        string Trace(string message, bool logToLogger = false);
        string Debug(string message, bool logToLogger = false);
        string Info(string message, bool logToLogger = false);
        string Warning(string message, bool logToLogger = true);
        string Error(string message, bool logToLogger = true);
        string Critical(string message, bool logToLogger = true);
        string Success(string message = "✔ Done", bool logToLogger = false);

        /// <summary>
        /// Logs a validation failure as an error message and optionally throws an exception.
        /// </summary>
        /// <param name="message">The validation failure message.</param>
        /// <param name="throwException">Whether to throw an exception after logging.</param>
        /// <param name="exceptionFactory">Optional factory for generating a custom exception type.</param>
        /// <returns>A failed <see cref="ValidationResult"/> instance containing the message.</returns>
        ValidationResult ErrorAndMaybeThrow(
            string message,
            bool throwException = false,
            Func<string, Exception>? exceptionFactory = null
        );
    }
}

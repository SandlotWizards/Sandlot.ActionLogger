using Sandlot.ActionLogger.Models;
using System;

namespace Sandlot.ActionLogger.Interfaces
{
    /// <summary>
    /// Defines a structured, observable logging service with step-based hierarchy,
    /// console coloring, structured log templates, and optional telemetry integration.
    /// </summary>
    public interface IActionLoggerService
    {
        /// <summary>
        /// Begins a new structured step in the logging sequence. This supports
        /// console output, structured log events, and OpenTelemetry activity spans.
        /// </summary>
        /// <param name="title">The human-readable name of the step.</param>
        /// <param name="threshold">Optional time threshold to trigger a warning log if exceeded.</param>
        /// <param name="logToLogger">If true, emits structured log output via <see cref="ILogger"/>.</param>
        /// <returns>A disposable context to end the step and record timing.</returns>
        IDisposable BeginStep(string title, TimeSpan? threshold = null, bool logToLogger = false);

        /// <summary>
        /// Logs a trace-level message. Optionally logs to the logger and/or throws an exception.
        /// </summary>
        string Trace(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true);

        /// <summary>
        /// Logs a debug-level message. Optionally logs to the logger and/or throws an exception.
        /// </summary>
        string Debug(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true);

        /// <summary>
        /// Logs an informational message. Optionally logs to the logger and/or throws an exception.
        /// </summary>
        string Info(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true);

        /// <summary>
        /// Logs a warning message. Optionally logs to the logger and/or throws an exception.
        /// </summary>
        string Warning(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true);

        /// <summary>
        /// Logs an error message. Optionally logs to the logger and/or throws an exception.
        /// </summary>
        string Error(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true);

        /// <summary>
        /// Logs a critical-level message. Optionally logs to the logger and/or throws an exception.
        /// </summary>
        string Critical(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true);

        /// <summary>
        /// Logs a success message. Optionally logs to the logger and/or throws an exception.
        /// </summary>
        string Success(string message = "✔ Done", bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = false);

        /// <summary>
        /// Logs a message with no prefix symbol. Supports optional coloring, logging, and exception throwing.
        /// </summary>
        string Message(string message, ConsoleColor color = ConsoleColor.Gray, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = false);

        /// <summary>
        /// Prints a formatted visual header to the console for the start of a logging section.
        /// </summary>
        void PrintHeader(ConsoleColor color = ConsoleColor.Gray);

        /// <summary>
        /// Prints a formatted visual trailer to the console for the end of a logging section.
        /// </summary>
        void PrintTrailer(ConsoleColor color = ConsoleColor.Gray);
    }
}

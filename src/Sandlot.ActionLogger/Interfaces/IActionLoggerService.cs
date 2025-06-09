namespace Sandlot.ActionLogger.Interfaces
{
    /// <summary>
    /// Provides structured step tracking with hierarchical step numbers,
    /// visual output to the console, and optional logging to ILogger.
    /// </summary>
    public interface IActionLoggerService
    {
        /// <summary>
        /// Begins a new tracked step with an optional duration threshold and logging option.
        /// </summary>
        /// <param name="title">The title of the step being executed.</param>
        /// <param name="threshold">Optional duration threshold. If exceeded, a warning is emitted.</param>
        /// <param name="logToLogger">Indicates whether to log this step to the logger.</param>
        /// <returns>An <see cref="IDisposable"/> context that ends the step when disposed.</returns>
        IDisposable BeginStep(string title, TimeSpan? threshold = null, bool logToLogger = false);

        /// <summary>
        /// Outputs a trace-level message to the console, and optionally to ILogger.
        /// </summary>
        /// <param name="message">The message to output.</param>
        /// <param name="logToLogger">Whether to log this message to ILogger.</param>
        /// <returns>The original message.</returns>
        string Trace(string message, bool logToLogger = false);

        /// <summary>
        /// Outputs a debug-level message to the console, and optionally to ILogger.
        /// </summary>
        string Debug(string message, bool logToLogger = false);

        /// <summary>
        /// Outputs an informational message to the console, and optionally to ILogger.
        /// </summary>
        string Info(string message, bool logToLogger = false);

        /// <summary>
        /// Outputs a warning message to the console, and optionally to ILogger.
        /// </summary>
        string Warning(string message, bool logToLogger = true);

        /// <summary>
        /// Outputs an error message to the console, and optionally to ILogger.
        /// </summary>
        string Error(string message, bool logToLogger = true);

        /// <summary>
        /// Outputs a critical failure message to the console, and optionally to ILogger.
        /// </summary>
        string Critical(string message, bool logToLogger = true);

        /// <summary>
        /// Outputs a success message to the console, and optionally to ILogger.
        /// </summary>
        string Success(string message = "✔ Done", bool logToLogger = false);
    }
}
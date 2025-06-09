using Microsoft.Extensions.Logging;
using Sandlot.ActionLogger.Interfaces;
using System.Diagnostics;

namespace Sandlot.ActionLogger.Services
{
    /// <summary>
    /// Default implementation of IStepTrackerService. Provides structured step tracking with hierarchical
    /// numbering, visual console output, threshold warnings, and optional structured logging.
    /// </summary>
    public class ActionLoggerService : IActionLoggerService
    {
        private readonly ILogger<ActionLoggerService>? _logger;
        private readonly Stack<string> _stepNumberStack = new();
        private int _currentStep = 0;
        private int _indentLevel = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionLoggerService"/> class.
        /// </summary>
        /// <param name="logger">Optional logger for structured output.</param>
        public ActionLoggerService(ILogger<ActionLoggerService>? logger = null)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public IDisposable BeginStep(string title, TimeSpan? threshold = null, bool logToLogger = false)
        {
            _currentStep++;
            _stepNumberStack.Push(_currentStep.ToString());
            var stepNumber = string.Join('.', _stepNumberStack.Reverse());
            WriteLine($"{stepNumber}. {title}", ConsoleColor.White, isMajor: true);
            if (logToLogger)
                _logger?.LogInformation("{Step} BEGIN: {Title}", stepNumber, title);

            return new StepContext(this, title, stepNumber, threshold, logToLogger);
        }

        /// <inheritdoc />
        public string Trace(string message, bool logToLogger = false)
        {
            WriteLine($"[trace] {message}", ConsoleColor.Cyan);
            if (logToLogger) _logger?.LogTrace(message);
            return message;
        }

        /// <inheritdoc />
        public string Debug(string message, bool logToLogger = false)
        {
            WriteLine($"[dbg] {message}", ConsoleColor.DarkGray);
            if (logToLogger) _logger?.LogDebug(message);
            return message;
        }

        /// <inheritdoc />
        public string Info(string message, bool logToLogger = false)
        {
            WriteLine($"⟳ {message}", ConsoleColor.Gray);
            if (logToLogger) _logger?.LogInformation(message);
            return message;
        }

        /// <inheritdoc />
        public string Warning(string message, bool logToLogger = true)
        {
            WriteLine($"⚠️ {message}", ConsoleColor.Yellow);
            if (logToLogger) _logger?.LogWarning(message);
            return message;
        }

        /// <inheritdoc />
        public string Error(string message, bool logToLogger = true)
        {
            WriteLine($"❌ {message}", ConsoleColor.Red);
            if (logToLogger) _logger?.LogError(message);
            return message;
        }

        /// <inheritdoc />
        public string Critical(string message, bool logToLogger = true)
        {
            WriteLine($"🔥 {message}", ConsoleColor.Red);
            if (logToLogger) _logger?.LogCritical(message);
            return message;
        }

        /// <inheritdoc />
        public string Success(string message = "✔ Done", bool logToLogger = false)
        {
            WriteLine(message, ConsoleColor.Green);
            if (logToLogger) _logger?.LogInformation(message);
            return message;
        }

        /// <summary>
        /// Outputs a message to the console with color and optional indentation.
        /// </summary>
        private void WriteLine(string message, ConsoleColor color, bool isMajor = false)
        {
            if (isMajor) Console.WriteLine();
            Console.ForegroundColor = color;
            Console.WriteLine(new string(' ', _indentLevel * 2) + message);
            Console.ResetColor();
        }

        /// <summary>
        /// Ends the current step, optionally logging its completion and duration.
        /// </summary>
        private void EndStep(string stepTitle, string stepNumber, Stopwatch stopwatch, TimeSpan? threshold, bool logToLogger)
        {
            var elapsed = stopwatch.Elapsed;
            var message = $"✔ {stepTitle} ({elapsed.TotalMilliseconds:N0}ms)";

            if (threshold.HasValue && elapsed > threshold.Value)
            {
                WriteLine($"⚠️ {message} — exceeded threshold", ConsoleColor.Yellow);
                if (logToLogger)
                    _logger?.LogWarning("{Step} SLOW: {Title} took {Elapsed}ms (threshold {Threshold}ms)", stepNumber, stepTitle, elapsed.TotalMilliseconds, threshold.Value.TotalMilliseconds);
            }
            else
            {
                WriteLine(message, ConsoleColor.Green);
                if (logToLogger)
                    _logger?.LogInformation("{Step} DONE: {Title} ({Elapsed}ms)", stepNumber, stepTitle, elapsed.TotalMilliseconds);
            }

            _stepNumberStack.Pop();
            _indentLevel--;
        }

        /// <summary>
        /// Represents a disposable step context. Calls back to the tracker upon disposal.
        /// </summary>
        private sealed class StepContext : IDisposable
        {
            private readonly ActionLoggerService _tracker;
            private readonly string _title;
            private readonly string _stepNumber;
            private readonly Stopwatch _stopwatch;
            private readonly TimeSpan? _threshold;
            private readonly bool _logToLogger;
            private bool _disposed;

            /// <summary>
            /// Initializes a new instance of the <see cref="StepContext"/> class.
            /// </summary>
            public StepContext(ActionLoggerService tracker, string title, string stepNumber, TimeSpan? threshold, bool logToLogger)
            {
                _tracker = tracker;
                _title = title;
                _stepNumber = stepNumber;
                _threshold = threshold;
                _logToLogger = logToLogger;
                _tracker._indentLevel++;
                _stopwatch = Stopwatch.StartNew();
            }

            /// <summary>
            /// Ends the step when the context is disposed.
            /// </summary>
            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                _stopwatch.Stop();
                _tracker.EndStep(_title, _stepNumber, _stopwatch, _threshold, _logToLogger);
            }
        }
    }
}
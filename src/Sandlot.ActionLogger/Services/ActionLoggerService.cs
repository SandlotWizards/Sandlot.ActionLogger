using Microsoft.Extensions.Logging;
using Sandlot.ActionLogger.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sandlot.ActionLogger.Services
{
    public class ActionLoggerService : IActionLoggerService
    {
        private static readonly ActivitySource _activitySource = new("Sandlot.ActionLogger");

        private readonly ILogger<ActionLoggerService>? _logger;
        private readonly Stack<string> _stepNumberStack = new();
        private int _currentStep = 0;
        private int _indentLevel = 0;

        public ActionLoggerService(ILogger<ActionLoggerService>? logger = null)
        {
            _logger = logger;
        }

        public IDisposable BeginStep(string title, TimeSpan? threshold = null, bool logToLogger = false)
        {
            _currentStep++;
            _stepNumberStack.Push(_currentStep.ToString());
            var stepNumber = string.Join('.', _stepNumberStack.Reverse());
            WriteLine($"{stepNumber}. {title}", ConsoleColor.White, isMajor: true);
            if (logToLogger)
                _logger?.LogInformation("{Step} BEGIN: {Title}", stepNumber, title);

            var activity = _activitySource.StartActivity(title, ActivityKind.Internal);
            return new StepContext(this, title, stepNumber, threshold, logToLogger, activity);
        }

        public string Trace(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true)
        {
            WriteLine($"[trace] {message}", ConsoleColor.Cyan);
            if (logToLogger) _logger?.LogTrace(message);
            if (throwException) throw exceptionFactory?.Invoke(message) ?? new Exception(message);
            return message;
        }

        public string Debug(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true)
        {
            WriteLine($"[dbg] {message}", ConsoleColor.DarkGray);
            if (logToLogger) _logger?.LogDebug(message);
            if (throwException) throw exceptionFactory?.Invoke(message) ?? new Exception(message);
            return message;
        }

        public string Info(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true)
        {
            WriteLine($"⟳ {message}", ConsoleColor.Gray);
            if (logToLogger) _logger?.LogInformation(message);
            if (throwException) throw exceptionFactory?.Invoke(message) ?? new Exception(message);
            return message;
        }

        public string Warning(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true)
        {
            WriteLine($"⚠️ {message}", ConsoleColor.Yellow);
            if (logToLogger) _logger?.LogWarning(message);
            if (throwException) throw exceptionFactory?.Invoke(message) ?? new Exception(message);
            return message;
        }

        public string Error(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true)
        {
            WriteLine($"❌ {message}", ConsoleColor.Red);
            if (logToLogger) _logger?.LogError(message);
            if (throwException) throw exceptionFactory?.Invoke(message) ?? new Exception(message);
            return message;
        }

        public string Critical(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true)
        {
            WriteLine($"🔥 {message}", ConsoleColor.Red);
            if (logToLogger) _logger?.LogCritical(message);
            if (throwException) throw exceptionFactory?.Invoke(message) ?? new Exception(message);
            return message;
        }

        public string Success(string message = "✔ Done", bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = false)
        {
            WriteLine(message, ConsoleColor.Green);
            if (logToLogger) _logger?.LogInformation(message);
            if (throwException) throw exceptionFactory?.Invoke(message) ?? new Exception(message);
            return message;
        }

        public string Message(string message, ConsoleColor color = ConsoleColor.Gray, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = false)
        {
            WriteLine(message, color);
            if (logToLogger) _logger?.LogInformation(message);
            if (throwException) throw exceptionFactory?.Invoke(message) ?? new Exception(message);
            return message;
        }

        public void PrintHeader(ConsoleColor color = ConsoleColor.Gray)
        {
            Console.WriteLine();
            Console.ForegroundColor = color;
            Console.WriteLine(new string('=', 120));
            Console.ResetColor();
        }

        public void PrintTrailer(ConsoleColor color = ConsoleColor.Gray)
        {
            Console.WriteLine(); // top blank line
            Console.ForegroundColor = color;
            Console.WriteLine(new string('=', 120));
            Console.ResetColor();
            Console.WriteLine(); // bottom blank line
        }

        private void WriteLine(string message, ConsoleColor color, bool isMajor = false)
        {
            if (isMajor) Console.WriteLine();
            Console.ForegroundColor = color;
            Console.WriteLine(new string(' ', _indentLevel * 2 + 1) + message);
            Console.ResetColor();
        }

        private void EndStep(string stepTitle, string stepNumber, Stopwatch stopwatch, TimeSpan? threshold, bool logToLogger, Activity? activity)
        {
            var elapsed = stopwatch.Elapsed;
            var message = $"✔ {stepTitle} ({elapsed.TotalMilliseconds:N0}ms)";

            if (threshold.HasValue && elapsed > threshold.Value)
            {
                WriteLine($"⚠️ {message} — exceeded threshold", ConsoleColor.Yellow);
                if (logToLogger)
                    _logger?.LogWarning("{Step} SLOW: {Title} took {Elapsed}ms (threshold {Threshold}ms)",
                        stepNumber, stepTitle, elapsed.TotalMilliseconds, threshold.Value.TotalMilliseconds);
            }
            else
            {
                WriteLine(message, ConsoleColor.Green);
                if (logToLogger)
                    _logger?.LogInformation("{Step} DONE: {Title} ({Elapsed}ms)", stepNumber, stepTitle, elapsed.TotalMilliseconds);
            }

            _stepNumberStack.Pop();
            _indentLevel--;

            activity?.SetTag("step.title", stepTitle);
            activity?.SetTag("step.number", stepNumber);
            activity?.SetTag("step.elapsedMs", elapsed.TotalMilliseconds);
            activity?.Stop();
        }

        private sealed class StepContext : IDisposable
        {
            private readonly ActionLoggerService _tracker;
            private readonly string _title;
            private readonly string _stepNumber;
            private readonly Stopwatch _stopwatch;
            private readonly TimeSpan? _threshold;
            private readonly bool _logToLogger;
            private readonly Activity? _activity;
            private bool _disposed;

            public StepContext(ActionLoggerService tracker, string title, string stepNumber, TimeSpan? threshold, bool logToLogger, Activity? activity)
            {
                _tracker = tracker;
                _title = title;
                _stepNumber = stepNumber;
                _threshold = threshold;
                _logToLogger = logToLogger;
                _activity = activity;
                _tracker._indentLevel++;
                _stopwatch = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                _stopwatch.Stop();
                _tracker.EndStep(_title, _stepNumber, _stopwatch, _threshold, _logToLogger, _activity);
            }
        }
    }
}
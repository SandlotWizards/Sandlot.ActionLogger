using Microsoft.Extensions.Logging;
using Sandlot.ActionLogger.Interfaces;
using Sandlot.ActionLogger.Models;
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
            if (logToLogger && _logger is not null)
                _logger.LogInformation("{Step} BEGIN: {Title}", stepNumber, title);

            var activity = _activitySource.StartActivity(title, ActivityKind.Internal);
            return new StepContext(this, title, stepNumber, threshold, logToLogger, activity);
        }

        public string Trace(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true)
        {
            if (logToLogger && _logger is not null) _logger.LogTrace(message);
            return Log("trace", message, ConsoleColor.Cyan, throwException, exceptionFactory);
        }

        public string Debug(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true)
        {
            if (logToLogger && _logger is not null) _logger.LogDebug(message);
            return Log("dbg", message, ConsoleColor.DarkGray, throwException, exceptionFactory);
        }

        public string Info(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true)
        {
            if (logToLogger && _logger is not null) _logger.LogInformation(message);
            return Log("⟳", message, ConsoleColor.Gray, throwException, exceptionFactory);
        }

        public string Warning(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true)
        {
            if (logToLogger && _logger is not null) _logger.LogWarning(message);
            return Log("⚠️", message, ConsoleColor.Yellow, throwException, exceptionFactory);
        }

        public string Error(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true)
        {
            if (logToLogger && _logger is not null) _logger.LogError(message);
            return Log("❌", message, ConsoleColor.Red, throwException, exceptionFactory);
        }

        public string Critical(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true)
        {
            if (logToLogger && _logger is not null) _logger.LogCritical(message);
            return Log("🔥", message, ConsoleColor.Red, throwException, exceptionFactory);
        }

        public string Success(string message = "✔ Done", bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = false)
        {
            if (logToLogger && _logger is not null) _logger.LogInformation(message);
            return Log("✔", message, ConsoleColor.Green, throwException, exceptionFactory);
        }

        public string Message(string message, ConsoleColor color = ConsoleColor.Gray, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = false)
        {
            WriteLine(message, color);
            if (logToLogger && _logger is not null) _logger.LogInformation(message);
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
            Console.WriteLine();
            Console.ForegroundColor = color;
            Console.WriteLine(new string('=', 120));
            Console.ResetColor();
            Console.WriteLine();
        }

        public ValidationResult TraceResult(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true) => ValidationResultFactory(Trace(message, throwException, exceptionFactory, logToLogger));
        public ValidationResult DebugResult(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true) => ValidationResultFactory(Debug(message, throwException, exceptionFactory, logToLogger));
        public ValidationResult InfoResult(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true) => ValidationResultFactory(Info(message, throwException, exceptionFactory, logToLogger));
        public ValidationResult WarningResult(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true) => ValidationResultFactory(Warning(message, throwException, exceptionFactory, logToLogger));
        public ValidationResult ErrorResult(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true) => ValidationResultFactory(Error(message, throwException, exceptionFactory, logToLogger), false);
        public ValidationResult CriticalResult(string message, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = true) => ValidationResultFactory(Critical(message, throwException, exceptionFactory, logToLogger), false);
        public ValidationResult SuccessResult(string message = "✔ Done", bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = false) => ValidationResultFactory(Success(message, throwException, exceptionFactory, logToLogger));
        public ValidationResult MessageResult(string message, ConsoleColor color = ConsoleColor.Gray, bool throwException = false, Func<string, Exception>? exceptionFactory = null, bool logToLogger = false) => ValidationResultFactory(Message(message, color, throwException, exceptionFactory, logToLogger));

        private string Log(string prefix, string message, ConsoleColor color, bool throwException, Func<string, Exception>? exceptionFactory)
        {
            WriteLine($"[{prefix}] {message}", color);
            if (throwException) throw exceptionFactory?.Invoke(message) ?? new Exception(message);
            return message;
        }

        private ValidationResult ValidationResultFactory(string message, bool isSuccess = true)
        {
            return isSuccess ? ValidationResult.Success() : ValidationResult.Fail(message);
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
                if (logToLogger && _logger is not null)
                    _logger.LogWarning("{Step} SLOW: {Title} took {Elapsed}ms (threshold {Threshold}ms)",
                        stepNumber, stepTitle, elapsed.TotalMilliseconds, threshold.Value.TotalMilliseconds);
            }
            else
            {
                WriteLine(message, ConsoleColor.Green);
                if (logToLogger && _logger is not null)
                    _logger.LogInformation("{Step} DONE: {Title} ({Elapsed}ms)", stepNumber, stepTitle, elapsed.TotalMilliseconds);
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
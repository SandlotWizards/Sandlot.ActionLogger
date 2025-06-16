using Sandlot.ActionLogger.Interfaces;
using System;

namespace Sandlot.ActionLogger
{
    public static class ActionLog
    {
        private static IActionLoggerService _instance;

        public static bool IsInitialized { get; private set; }

        public static void Initialize(IActionLoggerService service)
        {
            _instance = service;
        }

        public static IActionLoggerService Global =>
            _instance ?? throw new InvalidOperationException("ActionLogger is not initialized.");
    }
}

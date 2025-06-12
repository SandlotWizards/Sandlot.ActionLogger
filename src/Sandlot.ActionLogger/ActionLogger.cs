using Sandlot.ActionLogger.Interfaces;
using System;

namespace Sandlot.ActionLogger
{
    public static class ActionLogger
    {
        private static IActionLoggerService _instance;

        public static void Initialize(IActionLoggerService service)
        {
            _instance = service;
        }

        public static IActionLoggerService Global =>
            _instance ?? throw new InvalidOperationException("ActionLogger is not initialized.");
    }
}

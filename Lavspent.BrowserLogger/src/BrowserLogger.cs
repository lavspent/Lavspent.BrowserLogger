using Microsoft.Extensions.Logging;
using System;

namespace Lavspent.BrowserLogger
{

    class BrowserLogger : ILogger
    {
        private readonly BrowserLoggerService browserLoggerService;

        public BrowserLogger(BrowserLoggerService browserLoggerService)
        {
            this.browserLoggerService = browserLoggerService;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string message = formatter(state, exception);
            this.browserLoggerService.Enqueue(message+ "\r\n" + exception ?? "");
        }
    }
}
using Microsoft.Extensions.Logging;
using System;

namespace Lavspent.BrowserLogger
{

    class BrowserLogger : ILogger
    {
        private readonly BrowserLoggerQueue browserLoggerQueue;

        public BrowserLogger(BrowserLoggerQueue browserLoggerQueue)
        {
            this.browserLoggerQueue = browserLoggerQueue;
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
            this.browserLoggerQueue.Enqueue(message+ "\r\n" + exception ?? "");
        }
    }
}
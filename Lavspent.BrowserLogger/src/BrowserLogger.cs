using Microsoft.Extensions.Logging;
using System;

namespace Lavspent.BrowserLogger
{

    class BrowserLogger : ILogger
    {
        private readonly IBrowserLoggerService browserLoggerService;

        public BrowserLogger(IBrowserLoggerService browserLoggerService)
        {
            this.browserLoggerService = browserLoggerService;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            //throw new NotImplementedException();
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string message = formatter(state, exception);
            this.browserLoggerService.Write(message);
        }
    }

}
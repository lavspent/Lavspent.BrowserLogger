using Microsoft.Extensions.Logging;

namespace Lavspent.BrowserLogger
{
    public sealed class BrowserLoggerProvider : ILoggerProvider
    {
        private readonly BrowserLoggerService browserLoggerService;

        internal BrowserLoggerProvider(BrowserLoggerService browserLoggerService)
        {
            this.browserLoggerService = browserLoggerService;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new BrowserLogger(this.browserLoggerService);
        }

        public void Dispose()
        {
        }
    }
}
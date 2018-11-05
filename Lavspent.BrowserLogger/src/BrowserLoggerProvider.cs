using Microsoft.Extensions.Logging;

namespace Lavspent.BrowserLogger
{
    public class BrowserLoggerProvider : ILoggerProvider
    {
        private readonly IBrowserLoggerService browserLoggerService;

        internal BrowserLoggerProvider(IBrowserLoggerService browserLoggerService)
        {
            this.browserLoggerService = browserLoggerService;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new BrowserLogger(this.browserLoggerService);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
using Microsoft.Extensions.Logging;

namespace Lavspent.BrowserLogger
{
    public class BrowserLoggerProvider : ILoggerProvider
    {
        private readonly BrowserLoggerQueue browserLoggerQueue;

        internal BrowserLoggerProvider(BrowserLoggerQueue browserLoggerQueue)
        {
            this.browserLoggerQueue = browserLoggerQueue;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new BrowserLogger(this.browserLoggerQueue);
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
using Microsoft.Extensions.Logging;

namespace Lavspent.BrowserLogger
{
    public class BrowserLoggerProvider : ILoggerProvider
    {
        private readonly IBrowserLoggerService browserLoggerService;

        public BrowserLoggerProvider(IBrowserLoggerService browserLoggerService)
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

    public static class BrowserLoggerProviderExtensions
    {
        public static ILoggerFactory AddBrowserLoggerProvider(this ILoggerFactory loggerFactory)
        {
            //var serviceProvider = ServiceCollection.BuildServiceProvider();
            //var browserLoggerService = serviceProvider.GetService<IBrowserLoggerService>();

            //loggerFactory.AddProvider(new BrowserLoggerProvider(browserLoggerService));


            //loggerFactory.
            //return loggerFactory.AddProvider()
            return null;
        }
    }

}
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lavspent.BrowserLogger
{
    public static class BrowserLoggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseBrowserLogger(this IApplicationBuilder applicationBuilder)
        {
            // inject our logger
            var xdd = applicationBuilder.ApplicationServices.GetService<BrowserLoggerService>();
            var browserLoggerQueue = applicationBuilder.ApplicationServices.GetService<BrowserLoggerQueue>();
            var loggerFactory = applicationBuilder.ApplicationServices.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new BrowserLoggerProvider(browserLoggerQueue));

            // inject our middleware
            return applicationBuilder.UseMiddleware<BrowserLoggerMiddleware>();
        }
    }
}

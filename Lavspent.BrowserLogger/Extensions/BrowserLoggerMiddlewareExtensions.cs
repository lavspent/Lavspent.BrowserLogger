using Lavspent.BrowserLogger.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lavspent.BrowserLogger.Extensions
{
    public static class BrowserLoggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseBrowserLogger(this IApplicationBuilder applicationBuilder)
        {
            // inject our logger
            var browserLoggerService = applicationBuilder.ApplicationServices.GetService<BrowserLoggerService>();
            var loggerFactory = applicationBuilder.ApplicationServices.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new BrowserLoggerProvider(browserLoggerService));
            // inject our middleware
            return applicationBuilder.UseMiddleware<BrowserLoggerMiddleware>();
        }
    }
}
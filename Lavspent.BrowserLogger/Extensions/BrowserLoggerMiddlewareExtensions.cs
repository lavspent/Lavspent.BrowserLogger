using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lavspent.BrowserLogger.Extensions
{
    public static class BrowserLoggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseBrowserLogger(this IApplicationBuilder applicationBuilder)
        {
            var browserLoggerService = applicationBuilder.ApplicationServices.GetService<BrowserLoggerService>();
            var loggerFactory = applicationBuilder.ApplicationServices.GetService<ILoggerFactory>();

            loggerFactory.AddProvider(new BrowserLoggerProvider(browserLoggerService));
            return applicationBuilder.UseMiddleware<BrowserLoggerMiddleware>();
        }
    }
}
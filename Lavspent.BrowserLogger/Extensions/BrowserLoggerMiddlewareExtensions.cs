using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lavspent.BrowserLogger.Extensions
{
    public static class BrowserLoggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseBrowserLogger(this IApplicationBuilder app)
        {
            var browserLoggerService = app.ApplicationServices.GetService<BrowserLoggerService>();
            var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new BrowserLoggerProvider(browserLoggerService));
            return app.UseMiddleware<BrowserLoggerMiddleware>();
        }
    }
}
﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lavspent.BrowserLogger
{
    public static class BrowserLoggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseBrowserLogger(this IApplicationBuilder applicationBuilder, BrowserLoggerOptions options = null)
        {
            // inject our logger
            var browserLoggerService = applicationBuilder.ApplicationServices.GetService<BrowserLoggerService>();
            var loggerFactory = applicationBuilder.ApplicationServices.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new BrowserLoggerProvider(browserLoggerService));

            options = options ?? new BrowserLoggerOptions();
            

            // inject our middleware
            return applicationBuilder.UseMiddleware<BrowserLoggerMiddleware>(options);
        }
    }
}

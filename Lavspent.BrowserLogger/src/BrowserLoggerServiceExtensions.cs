using Microsoft.Extensions.DependencyInjection;

namespace Lavspent.BrowserLogger
{
    public static class BrowserLoggerServiceExtensions
    {
        public static IServiceCollection AddBrowserLogger(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<BrowserLoggerQueue>()
                .AddHostedService<BrowserLoggerService>();
        }
    }
}
using Microsoft.Extensions.DependencyInjection;

namespace Lavspent.BrowserLogger.Extensions
{
    public static class BrowserLoggerServiceExtensions
    {
        public static IServiceCollection AddBrowserLogger(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<BrowserLoggerService>();
        }
    }
}
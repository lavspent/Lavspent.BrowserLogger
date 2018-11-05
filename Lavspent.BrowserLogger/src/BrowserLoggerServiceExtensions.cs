using Microsoft.Extensions.DependencyInjection;

namespace Lavspent.BrowserLogger
{
    public static class BrowserLoggerServiceExtensions
    {
        public static IServiceCollection AddBrowserLoggerService(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSingleton<IBrowserLoggerService>(new BrowserLoggerService());
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Lavspent.BrowserLogger
{
    public class BrowserLoggerService : IBrowserLoggerService
    {
        private ConcurrentDictionary<WebSocket, WebSocket> _webSockets = new ConcurrentDictionary<WebSocket, WebSocket>();

        public BrowserLoggerService()
        {
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <returns></returns>
        public IDisposable RegisterWebSocket(WebSocket webSocket)
        {
            //var stream = new Stream();
            //var streamWriter = new StreamWriter(outputStream);
            if (!_webSockets.TryAdd(webSocket, webSocket))
            {
                throw new Exception("Cound't register websocket.");
            }

            return new RegistrationDisposer(this, webSocket);
        }

        private class RegistrationDisposer : IDisposable
        {
            private readonly BrowserLoggerService browserLoggerService;
            private readonly WebSocket webSocket;

            public RegistrationDisposer(BrowserLoggerService browserLoggerService, WebSocket webSocket)
            {
                this.browserLoggerService = browserLoggerService ?? throw new ArgumentNullException(nameof(browserLoggerService));
                this.webSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
            }

            public void Dispose()
            {
                browserLoggerService._webSockets.TryRemove(webSocket, out var _);
            }
        }

        public void Write(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);

            foreach (var sw in _webSockets.Keys.ToArray())   // We need toArray here?
            {
                try
                {
                    sw.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None).Wait();
                } catch (Exception e)
                {
                    try
                    {
                        _webSockets.TryRemove(sw, out var _);
                    } catch
                    {
                        // ignore
                    }
                }
            }
        }
    }


    public static class BrowserLoggerServiceExtensions
    {
        public static IServiceCollection AddBrowserLoggerService(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSingleton<IBrowserLoggerService>(new BrowserLoggerService());
        }
    }

}
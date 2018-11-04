using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Lavspent.BrowserLogger
{
    class BrowserLoggerMiddleware
    {
    }

    public static class BrowserLoggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseBrowserLogger(this IApplicationBuilder applicationBuilder)
        {
            // inject our logger
            var browserLoggerService = applicationBuilder.ApplicationServices.GetService<IBrowserLoggerService>();
            var loggerFactory = applicationBuilder.ApplicationServices.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new BrowserLoggerProvider(browserLoggerService));

            // inject our middleware
            applicationBuilder.Use(async (context, next) =>
                {

                    if (context.Request.Path == "/api/server-health/logstream")
                    {
                        if (context.WebSockets.IsWebSocketRequest)
                        {
                            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                            IBrowserLoggerService bls = applicationBuilder.ApplicationServices.GetService<IBrowserLoggerService>();
                            var dispoable = bls.RegisterWebSocket(webSocket);

                            byte[] buf = new byte[4095];
                            while (webSocket.State == WebSocketState.Open)
                            {
                                var result = await webSocket.ReceiveAsync(buf, CancellationToken.None);

                                if (result.MessageType == WebSocketMessageType.Close)
                                {
                                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                                    dispoable.Dispose();
                                }
                                else
                                {
                                    // nop
                                }
                            }
                        }
                        else
                        {
                            context.Response.StatusCode = 400;
                        }
                    }
                    else
                    {
                        await next();
                    }
                });

            return applicationBuilder;
        }
    }
}

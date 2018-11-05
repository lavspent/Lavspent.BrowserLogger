using Microsoft.AspNetCore.Builder;
using System.Net.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Lavspent.BrowserLogger
{
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
                    if (context.Request.Path == "/console")
                    {
                        var assembly = Assembly.GetExecutingAssembly();
                        //var names = assembly.GetManifestResourceNames();
                        var resourceStream = assembly.GetManifestResourceStream("Lavspent.BrowserLogger.src.BrowserLoggerConsole.html");
                        context.Response.StatusCode = 200;
                        context.Response.ContentType = "text/html";
                        await resourceStream.CopyToAsync(context.Response.Body);
                        //return File(resourceStream, "text/html");
                    } 
                    else if (context.Request.Path == "/logstream")
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

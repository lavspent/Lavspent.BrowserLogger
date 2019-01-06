using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Reflection;
using System.Threading.Tasks;

namespace Lavspent.BrowserLogger
{
    internal class BrowserLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly BrowserLoggerService _browserLoggerService;


        public BrowserLoggerMiddleware(RequestDelegate next, BrowserLoggerService browserLoggerService)
        {
            _next = next;
            _browserLoggerService = browserLoggerService;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path == "/console")
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceStream = assembly.GetManifestResourceStream("Lavspent.BrowserLogger.src.BrowserLoggerConsole.html");
                httpContext.Response.StatusCode = 200;
                httpContext.Response.ContentType = "text/html";
                await resourceStream.CopyToAsync(httpContext.Response.Body);
            }
            else if (httpContext.Request.Path == "/logstream")
            {
                if (httpContext.WebSockets.IsWebSocketRequest)
                {
                    WebSocket webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();

                    using (var registration = _browserLoggerService.RegisterWebSocket(webSocket))
                    {
                        await registration.HandleIOAsync(httpContext.RequestAborted);
                    }
                }
                else
                {
                    httpContext.Response.StatusCode = 400;
                }
            }
            else
            {
                await _next.Invoke(httpContext);
            }
        }
    }
}

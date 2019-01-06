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
                await HandleConsoleRequest(httpContext);
            }
            else if (httpContext.Request.Path == "/logstream")
            {
                await HandleConsoleRequest(httpContext);
            }
            else
            {
                await _next.Invoke(httpContext);
            }
        }

        private async Task HandleConsoleRequest(HttpContext httpContext)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceStream = assembly.GetManifestResourceStream("Lavspent.BrowserLogger.src.BrowserLoggerConsole.html");
            httpContext.Response.StatusCode = 200;
            httpContext.Response.ContentType = "text/html";
            await resourceStream.CopyToAsync(httpContext.Response.Body);
        }

        private async Task HandleLogStreamRequest(HttpContext httpContext)
        {
            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();

                using (var registration = _browserLoggerService.RegisterWebSocket(webSocket))
                {
                    try
                    {
                        await registration.HandleIOAsync(httpContext.RequestAborted);
                    }
                    catch when (httpContext.RequestAborted.IsCancellationRequested)
                    {
                        // ignore
                    }
                }
            }
            else
            {
                httpContext.Response.StatusCode = 400;
            }
        }
    }
}

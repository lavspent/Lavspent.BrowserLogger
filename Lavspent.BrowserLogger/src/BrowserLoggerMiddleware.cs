using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.WebSockets;
using System.Reflection;
using System.Threading.Tasks;

namespace Lavspent.BrowserLogger
{

    internal class BrowserLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly BrowserLoggerService _browserLoggerService;
        private readonly BrowserLoggerOptions _options;

        public BrowserLoggerMiddleware(RequestDelegate next, BrowserLoggerService browserLoggerService, BrowserLoggerOptions options)
        {
            _next = next;
            _browserLoggerService = browserLoggerService;
            _options = options;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path == this._options.ConsolePath)
            {
                await HandleConsoleRequest(httpContext);
            }
            else if (httpContext.Request.Path == this._options.LogStreamPath)
            {
                await HandleLogStreamRequest(httpContext);
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

            string options = "{wsUri: \"ws://\" + location.host + \"" + _options.LogStreamPath + "\"}";
            string init = "<script language=\"javascript\" type=\"text/javascript\">init(" + options + ");</script>";
            init += "</body></html>";

            using (StreamWriter streamWriter = new StreamWriter(httpContext.Response.Body))
            {
                streamWriter.WriteLine();
                streamWriter.Write(init);
                streamWriter.Flush();
            }
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

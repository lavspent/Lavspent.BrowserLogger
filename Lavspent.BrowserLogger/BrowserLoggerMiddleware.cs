﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Lavspent.BrowserLogger.Extensions;
using Lavspent.BrowserLogger.Options;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lavspent.BrowserLogger
{
    internal class BrowserLoggerMiddleware
    {
        private readonly BrowserLoggerService _browserLoggerService;
        private readonly Uri _consoleUri;
        private readonly Uri _logStreamUri;
        private readonly RequestDelegate _next;
        private BrowserLoggerOptions _options;

        public BrowserLoggerMiddleware(RequestDelegate next, BrowserLoggerService browserLoggerService,
            BrowserLoggerOptions options)
        {
            _next = next;
            _browserLoggerService = browserLoggerService;
            _options = options;
            _logStreamUri = new Uri(options.LogStreamUrl);
            _consoleUri = new Uri(_logStreamUri, options.ConsolePath);
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path.Value == _consoleUri.AbsolutePath)
                await HandleConsoleRequest(httpContext);
            else if (httpContext.Request.Path.Value == _logStreamUri.AbsolutePath)
                await HandleLogStreamRequest(httpContext);
            else
                await _next.Invoke(httpContext);
        }

        private async Task HandleConsoleRequest(HttpContext httpContext)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceStream =
                assembly.GetManifestResourceStream("Lavspent.BrowserLogger.Templates.Default.html");
            httpContext.Response.StatusCode = 200;
            httpContext.Response.ContentType = "text/html";
            var content = resourceStream.ReadString();
            SetIniScript(ref content);

            await httpContext.Response.WriteAsync(content);
        }

        private void SetIniScript(ref string content)
        {
            // Preparing Initialization script
            var options = new ScriptOptions(_options);
            var optionsJson = JsonConvert.SerializeObject(options,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            var initScript = "<script language='javascript' type='text/javascript'>\n" +
                             $"init({optionsJson});\n</script>\n" +
                             "</body>";

            content = content.Replace("</body>", initScript,
                StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task HandleLogStreamRequest(HttpContext httpContext)
        {
            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();

                using (var registration = _browserLoggerService.RegisterWebSocket(webSocket))
                {
                    try
                    {
                        await registration.HandleIoAsync(httpContext.RequestAborted);
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
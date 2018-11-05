using System;
using System.IO;
using System.Net.WebSockets;

namespace Lavspent.BrowserLogger
{
    internal interface IBrowserLoggerService
    {
        IDisposable RegisterWebSocket(WebSocket webSocket);
        void Write(string value);
    }
}
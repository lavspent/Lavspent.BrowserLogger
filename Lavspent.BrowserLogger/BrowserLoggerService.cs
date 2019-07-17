using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using Lavspent.BrowserLogger.Handlers;
using Lavspent.BrowserLogger.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lavspent.BrowserLogger
{
    internal class BrowserLoggerService
    {
        private readonly ConcurrentDictionary<WebSocket, WebSocketHandler> _registrations =
            new ConcurrentDictionary<WebSocket, WebSocketHandler>();

        public WebSocketHandler RegisterWebSocket(WebSocket webSocket)
        {
            var registration = new WebSocketHandler(this, webSocket);
            ((IDictionary<WebSocket, WebSocketHandler>) _registrations).Add(webSocket, registration);
            return registration;
        }

        public void UnRegisterWebSocket(WebSocket webSocket)
        {
            ((IDictionary<WebSocket, WebSocketHandler>) _registrations).Remove(webSocket);
        }

        public void Enqueue(LogMessageEntry logMessageEntry)
        {
            var settings = new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};
            var serializedEntry = JsonConvert.SerializeObject(logMessageEntry, settings);
            var bytes = Encoding.UTF8.GetBytes(serializedEntry);
            foreach (var sw in _registrations.Values) sw.Queue.Enqueue(bytes);
        }
    }
}
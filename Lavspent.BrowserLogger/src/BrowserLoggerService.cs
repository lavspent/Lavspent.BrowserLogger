using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace Lavspent.BrowserLogger
{
    internal class BrowserLoggerService
    {
        private readonly ConcurrentDictionary<WebSocket, WebSocketHandler> _registrations = new ConcurrentDictionary<WebSocket, WebSocketHandler>();
        private readonly AsyncQueue<string> _browserLoggerQueue = new AsyncQueue<string>();

        public BrowserLoggerService()
        {
        }

        public WebSocketHandler RegisterWebSocket(WebSocket webSocket)
        {
            var registration = new WebSocketHandler(this, webSocket);
            ((IDictionary<WebSocket, WebSocketHandler>)_registrations).Add(webSocket, registration);
            return registration;
        }

        public void UnregisterWebSocket(WebSocket webSocket)
        {
            ((IDictionary<WebSocket, WebSocketHandler>)_registrations).Remove(webSocket);
        }

        public void Enqueue(LogMessageEntry logMessageEntry)
        {
            // TODO: Reuse settings?
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();


            string serializedEntry = JsonConvert.SerializeObject(logMessageEntry, settings);

            // convert data to bytes
            byte[] bytes = Encoding.UTF8.GetBytes(serializedEntry);

            // add to all listeners
            foreach (var sw in _registrations.Values)
            {
                sw._queue.Enqueue(bytes);
            }
        }
    }
}

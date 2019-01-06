using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace Lavspent.BrowserLogger
{
    internal class BrowserLoggerService
    {
        internal readonly ConcurrentDictionary<WebSocket, WebSocketRegistration> _registrations = new ConcurrentDictionary<WebSocket, WebSocketRegistration>();
        private readonly AsyncQueue<string> _browserLoggerQueue = new AsyncQueue<string>();

        public BrowserLoggerService()
        {
        }

        public WebSocketRegistration RegisterWebSocket(WebSocket webSocket)
        {
            var registration = new WebSocketRegistration(this, webSocket);
            ((IDictionary<WebSocket, WebSocketRegistration>)_registrations).Add(webSocket, registration);
            return registration;
        }

        public void UnregisterWebSocket(WebSocket webSocket)
        {
            ((IDictionary<WebSocket, WebSocketRegistration>)_registrations).Remove(webSocket);
        }

        public void Enqueue(string value)
        {
            // convert data to bytes
            byte[] bytes = Encoding.UTF8.GetBytes(value);

            // add to all listeners
            foreach (var sw in _registrations.Values)
            {
                sw._queue.Enqueue(bytes);
            }
        }
    }
}

using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.BrowserLogger
{
    internal partial class BrowserLoggerService : BackgroundService//, IBrowserLoggerService
    {
        internal readonly ConcurrentDictionary<WebSocket, WebSocketRegistration> _registrations = new ConcurrentDictionary<WebSocket, WebSocketRegistration>();
        private readonly BrowserLoggerQueue _browserLoggerQueue;

        public BrowserLoggerService(BrowserLoggerQueue browserLoggerQueue)
        {
            this._browserLoggerQueue = browserLoggerQueue;
        }

        internal WebSocketRegistration RegisterWebSocket(WebSocket webSocket)
        {
            var registration = new WebSocketRegistration(this, webSocket);
            ((IDictionary<WebSocket, WebSocketRegistration>)_registrations).Add(webSocket, registration);
            return registration;
        }

        internal void UnregisterWebSocket(WebSocket webSocket)
        {
            ((IDictionary<WebSocket, WebSocketRegistration>)_registrations).Remove(webSocket);
        }

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                string loggedItem = await _browserLoggerQueue.DequeueAsync(cancellationToken);

                // convert data to bytes
                byte[] bytes = Encoding.UTF8.GetBytes(loggedItem);

                // add to all listening sockets
                foreach (var sw in _registrations.Values)
                {
                    sw._queue.Enqueue(bytes);
                }
            }
        }
    }
}

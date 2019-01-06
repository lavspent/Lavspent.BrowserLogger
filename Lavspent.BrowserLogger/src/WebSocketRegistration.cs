using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.BrowserLogger
{
    internal class WebSocketRegistration : IDisposable
    {
        private readonly BrowserLoggerService _browserLoggerService;
        private readonly WebSocket _webSocket;
        internal readonly AsyncQueue<byte[]> _queue;

        internal WebSocketRegistration(BrowserLoggerService browserLoggerService, WebSocket webSocket)
        {
            this._browserLoggerService = browserLoggerService ?? throw new ArgumentNullException(nameof(browserLoggerService));
            this._webSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
            this._queue = new AsyncQueue<byte[]>();
        }

        public void Dispose()
        {
            _browserLoggerService.UnregisterWebSocket(_webSocket);
        }

        public async Task HandleIOAsync(CancellationToken cancellationToken)
        {
            byte[] buf = new byte[4095];
            Task<WebSocketReceiveResult> receiveTask = null;
            Task<byte[]> dequeueTask = null;

            while (_webSocket.State == WebSocketState.Open)
            {
                // wait for data in either direction
                receiveTask = receiveTask ?? _webSocket.ReceiveAsync(buf, cancellationToken);
                dequeueTask = dequeueTask ?? _queue.DequeueAsync(cancellationToken);
                var i = Task.WaitAny(new Task[] { receiveTask, dequeueTask}, cancellationToken);

                if (i == 0)
                {
                    await HandleReceive(receiveTask.Result, cancellationToken);
                    receiveTask = null;
                } else
                {
                    await HandleSend(dequeueTask.Result, cancellationToken);
                    dequeueTask = null;
                }
            }
        }

        private async Task HandleReceive(WebSocketReceiveResult result, CancellationToken cancellationToken)
        {
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", cancellationToken);
            }
        }

        private async Task HandleSend(byte[] result, CancellationToken cancellationToken)
        {
            await _webSocket.SendAsync(result, WebSocketMessageType.Text, true, cancellationToken);
        }
    }
}

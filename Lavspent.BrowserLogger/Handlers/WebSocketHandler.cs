using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Lavspent.BrowserLogger.Api;

namespace Lavspent.BrowserLogger.Handlers
{
    internal class WebSocketHandler : IDisposable
    {
        private readonly BrowserLoggerService _browserLoggerService;
        private readonly WebSocket _webSocket;

        internal WebSocketHandler(BrowserLoggerService browserLoggerService, WebSocket webSocket)
        {
            _browserLoggerService =
                browserLoggerService ?? throw new ArgumentNullException(nameof(browserLoggerService));
            _webSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
            Queue = new AsyncQueue<byte[]>();
        }

        internal AsyncQueue<byte[]> Queue { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (disposing) _browserLoggerService.UnRegisterWebSocket(_webSocket);
        }

        public async Task HandleIoAsync(CancellationToken cancellationToken)
        {
            var buf = new byte[4095];
            Task<WebSocketReceiveResult> receiveTask = null;
            Task<byte[]> dequeueTask = null;

            while (_webSocket.State == WebSocketState.Open)
            {
                // wait for data in either direction
                receiveTask = receiveTask ?? _webSocket.ReceiveAsync(buf, cancellationToken);
                dequeueTask = dequeueTask ?? Queue.DequeueAsync(cancellationToken);
                var i = Task.WaitAny(new Task[] {receiveTask, dequeueTask}, cancellationToken);

                if (i == 0)
                {
                    await HandleReceive(receiveTask.Result, cancellationToken);
                    receiveTask = null;
                }
                else
                {
                    await HandleSend(dequeueTask.Result, cancellationToken);
                    dequeueTask = null;
                }
            }
        }

        private async Task HandleReceive(WebSocketReceiveResult result, CancellationToken cancellationToken)
        {
            if (result.MessageType == WebSocketMessageType.Close)
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", cancellationToken);
        }

        private async Task HandleSend(byte[] result, CancellationToken cancellationToken)
        {
            await _webSocket.SendAsync(result, WebSocketMessageType.Text, true, cancellationToken);
        }
    }
}
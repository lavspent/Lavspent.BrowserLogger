using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Lavspent.BrowserLogger.Api
{
    internal class AsyncQueue<TType>
    {
        private readonly ConcurrentQueue<TType> _queue = new ConcurrentQueue<TType>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void Enqueue(TType value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            _queue.Enqueue(value);
            _signal.Release();
        }

        public async Task<TType> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _queue.TryDequeue(out var workItem);
            return workItem;
        }
    }
}
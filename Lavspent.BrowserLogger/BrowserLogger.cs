using System;
using Lavspent.BrowserLogger.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;

namespace Lavspent.BrowserLogger
{
    internal class BrowserLogger : ILogger
    {
        private readonly BrowserLoggerService _browserLoggerService;
        private Func<string, LogLevel, bool> _filter;

        public BrowserLogger(string name, Func<string, LogLevel, bool> filter,
            BrowserLoggerService browserLoggerService)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Filter = filter ?? ((category, logLevel) => true);
            _browserLoggerService =
                browserLoggerService ?? throw new ArgumentNullException(nameof(browserLoggerService));
        }

        public string Name { get; }

        public Func<string, LogLevel, bool> Filter
        {
            get => _filter;
            set => _filter = value ?? throw new ArgumentNullException(nameof(value));
        }

        internal IExternalScopeProvider ScopeProvider { get; set; }

        public IDisposable BeginScope<TState>(TState state)
        {
            return ScopeProvider?.Push(state) ?? NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            var message = formatter(state, exception);

            if (!string.IsNullOrEmpty(message) || exception != null)
                _browserLoggerService.Enqueue(new LogMessageEntry
                {
                    LogLevel = logLevel,
                    TimeStampUtc = DateTime.UtcNow,
                    Name = Name,
                    Message = message
                });
        }
    }
}
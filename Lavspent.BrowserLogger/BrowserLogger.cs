using System;
using System.Text;
using Lavspent.BrowserLogger.Models;
using Lavspent.BrowserLogger.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using Microsoft.Extensions.Options;

namespace Lavspent.BrowserLogger
{
    internal class BrowserLogger : ILogger
    {
        private readonly BrowserLoggerService _browserLoggerService;
        private readonly BrowserLoggerOptions _options;
        private Func<string, LogLevel, bool> _filter;

        public BrowserLogger(string name, Func<string, LogLevel, bool> filter,
            BrowserLoggerService browserLoggerService, IOptions<BrowserLoggerOptions> options)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Filter = filter ?? ((category, logLevel) => true);
            _browserLoggerService =
                browserLoggerService ?? throw new ArgumentNullException(nameof(browserLoggerService));

            _options = options?.Value ?? new BrowserLoggerOptions();
        }

        public string Name { get; }

        public Func<string, LogLevel, bool> Filter
        {
            get => _filter;
            set => _filter = value ?? throw new ArgumentNullException(nameof(value));
        }

        internal IExternalScopeProvider ScopeProvider { get; set; }

        public bool DisableColors { get; set; }

        internal LogLevel LogToStandardErrorThreshold { get; set; }

        internal string TimestampFormat { get; set; }


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
                    LogLevel = GetLogLevelString(logLevel),
                    TimeStampUtc = DateTime.UtcNow,
                    Name = Name,
                    Message = message,
                    LogAsError = logLevel >= LogToStandardErrorThreshold
                });
        }

        private static string GetLogLevelString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return "trce";
                case LogLevel.Debug:
                    return "dbug";
                case LogLevel.Information:
                    return "info";
                case LogLevel.Warning:
                    return "warn";
                case LogLevel.Error:
                    return "fail";
                case LogLevel.Critical:
                    return "crit";
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        private void GetScopeInformation(StringBuilder stringBuilder)
        {
            var scopeProvider = ScopeProvider;
            if (scopeProvider != null)
            {
                var initialLength = stringBuilder.Length;

                scopeProvider.ForEachScope((scope, state) =>
                {
                    var (builder, length) = state;
                    var first = length == builder.Length;
                    builder.Append(" => ").Append(scope);
                }, (stringBuilder, initialLength));

                if (stringBuilder.Length > initialLength) stringBuilder.AppendLine();
            }
        }
    }
}
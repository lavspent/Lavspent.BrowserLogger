using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Lavspent.BrowserLogger
{

    class BrowserLogger : ILogger
    {
        private static readonly string _loglevelPadding = ": ";
        private static readonly string _messagePadding;
        private static readonly string _newLineWithMessagePadding;

        private readonly BrowserLoggerService browserLoggerService;

        public string Name { get; }

        private Func<string, LogLevel, bool> _filter;
        public Func<string, LogLevel, bool> Filter
        {
            get { return _filter; }
            set
            {
                _filter = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        internal IExternalScopeProvider ScopeProvider { get; set; }

        public bool DisableColors { get; set; }

        internal LogLevel LogToStandardErrorThreshold { get; set; }

        internal string TimestampFormat { get; set; }

        static BrowserLogger()
        {
            var logLevelString = GetLogLevelString(LogLevel.Information);
            _messagePadding = String.Concat(Enumerable.Repeat("&nbsp;", logLevelString.Length + _loglevelPadding.Length));
            _newLineWithMessagePadding = "<br/>" + _messagePadding;
        }

        public BrowserLogger(string name, Func<string, LogLevel, bool> filter, BrowserLoggerService browserLoggerService)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Filter = filter ?? ((category, logLevel) => true);
            this.browserLoggerService = browserLoggerService ?? throw new ArgumentNullException(nameof(browserLoggerService));

        }


        public IDisposable BeginScope<TState>(TState state) => ScopeProvider?.Push(state) ?? NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        //        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        //        {
        //            string message = formatter(state, exception);
        //            this.browserLoggerService.Enqueue(message+ "\r\n" + exception ?? "");
        //        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                WriteMessage(logLevel, Name, eventId.Id, message, exception);
            }
        }

        public virtual void WriteMessage(LogLevel logLevel, string logName, int eventId, string message, Exception exception)
        {
            //var logBuilder = _logBuilder;
            //_logBuilder = null;

            //if (logBuilder == null)
            //{
            //    logBuilder = new StringBuilder();
            //}
            var logBuilder = new StringBuilder();


            var logLevelColors = default(BrowserColors);
            var logLevelString = string.Empty;

            // Example:
            // INFO: ConsoleApp.Program[10]
            //       Request received

            logLevelColors = GetLogLevelBrowserColors(logLevel);
            logLevelString = GetLogLevelString(logLevel);

            // category and event id
            logBuilder.Append(_loglevelPadding);
            logBuilder.Append(logName);
            logBuilder.Append("[");
            logBuilder.Append(eventId);
            logBuilder.AppendLine("]");

            // scope information
            GetScopeInformation(logBuilder);

            if (!string.IsNullOrEmpty(message))
            {
                // message
                logBuilder.Append(_messagePadding);

                var len = logBuilder.Length;
                logBuilder.AppendLine(message);
                var z = logBuilder.ToString();

                logBuilder.Replace(Environment.NewLine, _newLineWithMessagePadding, len, message.Length);

                var x = logBuilder.ToString();
//                Debugger.Break();
            }

            // Example:
            // System.InvalidOperationException
            //    at Namespace.Class.Function() in File:line X
            if (exception != null)
            {
                // exception message
                logBuilder.AppendLine(exception.ToString());
            }

            var hasLevel = !string.IsNullOrEmpty(logLevelString);
            var timestampFormat = TimestampFormat;
            // Queue log message


            //            this.browserLoggerService.Enqueue(message+ "\r\n" + exception ?? "");

            this.browserLoggerService.Enqueue(new LogMessageEntry()
            {
                TimeStamp = timestampFormat != null ? DateTime.Now.ToString(timestampFormat) : null,
                Message = logBuilder.ToString(),
                MessageColor = BrowserColor.White.Color, // TODO?! DefaultConsoleColor,
                LevelString = hasLevel ? logLevelString : null,
                LevelBackground = hasLevel ? logLevelColors.Background.Color : null,
                LevelForeground = hasLevel ? logLevelColors.Foreground.Color : null,
                LogAsError = logLevel >= LogToStandardErrorThreshold
            });

            //logBuilder.Clear();
            //if (logBuilder.Capacity > 1024)
            //{
            //    logBuilder.Capacity = 1024;
            //}
            //_logBuilder = logBuilder;
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


        private BrowserColors GetLogLevelBrowserColors(LogLevel logLevel)
        {
            if (DisableColors)
            {
                return BrowserColors.Default; 
            }

            // We must explicitly set the background color if we are setting the foreground color,
            // since just setting one can look bad on the users console.
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return new BrowserColors(BrowserColor.White, BrowserColor.Red);
                case LogLevel.Error:
                    return new BrowserColors(BrowserColor.Black, BrowserColor.Red);
                case LogLevel.Warning:
                    return new BrowserColors(BrowserColor.Yellow, BrowserColor.Black);
                case LogLevel.Information:
                    return new BrowserColors(BrowserColor.DarkGreen, BrowserColor.Black);
                case LogLevel.Debug:
                    return new BrowserColors(BrowserColor.Gray, BrowserColor.Black);
                case LogLevel.Trace:
                    return new BrowserColors(BrowserColor.Gray, BrowserColor.Black);
                default:
                    return BrowserColors.Default;
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
                    builder.Append(first ? "=> " : " => ").Append(scope);
                }, (stringBuilder, initialLength));

                if (stringBuilder.Length > initialLength)
                {
                    stringBuilder.Insert(initialLength, _messagePadding);
                    stringBuilder.AppendLine();
                }
            }
        }

    }


    public class BrowserColor
    {
        public static readonly BrowserColor Black = new BrowserColor("black");
        public static readonly BrowserColor DarkBlue = new BrowserColor("darkblue");
        public static readonly BrowserColor DarkGreen = new BrowserColor("darkgreen");
        public static readonly BrowserColor DarkCyan = new BrowserColor("darkcyan");
        public static readonly BrowserColor DarkRed = new BrowserColor("darkred");
        public static readonly BrowserColor DarkMagenta = new BrowserColor("darkmagenta");
        public static readonly BrowserColor DarkYellow = new BrowserColor("darkyellow");
        public static readonly BrowserColor Gray = new BrowserColor("Gray");
        public static readonly BrowserColor DarkGray = new BrowserColor("darkgray");
        public static readonly BrowserColor Blue = new BrowserColor("blue");
        public static readonly BrowserColor Green = new BrowserColor("green");
        public static readonly BrowserColor Cyan = new BrowserColor("cyan");
        public static readonly BrowserColor Red = new BrowserColor("red");
        public static readonly BrowserColor Magenta = new BrowserColor("magenta");
        public static readonly BrowserColor Yellow = new BrowserColor("yellow");
        public static readonly BrowserColor White = new BrowserColor("white");

        public string Color { get; private set; }

        public BrowserColor(string color)
        {
            Color = color;
        }
    }

    internal class BrowserColors
    {
        // TODO: Maybe I shouldn't know this?
        public static readonly BrowserColors Default = new BrowserColors(BrowserColor.White, BrowserColor.Black);

        public BrowserColors(BrowserColor foreground, BrowserColor background)
        {
            Foreground = foreground;
            Background = background;
        }

        public BrowserColor Foreground { get; }

        public BrowserColor Background { get; }
    }
}

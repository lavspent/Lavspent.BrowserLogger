using Microsoft.Extensions.Logging.Console;

namespace Lavspent.BrowserLogger
{
    public class BrowserLoggerOptions : ConsoleLoggerOptions
    {
        public string ConsolePath { get; set; } = "/console";
        public string LogStreamPath { get; set; } = "/logstream";
    }
}
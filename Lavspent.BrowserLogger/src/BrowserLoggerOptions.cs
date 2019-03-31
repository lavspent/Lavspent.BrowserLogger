using Microsoft.Extensions.Logging.Console;

namespace Lavspent.BrowserLogger
{
    public class BrowserLoggerOptions : ConsoleLoggerOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public string ConsolePath { get; set; } = "/console";

        /// <summary>
        /// 
        /// </summary>
        public string LogStreamPath { get; set; } = "/logstream";
    }
}

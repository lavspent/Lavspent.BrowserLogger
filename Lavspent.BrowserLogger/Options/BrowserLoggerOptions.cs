using Microsoft.Extensions.Logging.Console;

namespace Lavspent.BrowserLogger.Options
{
    public class BrowserLoggerOptions : ConsoleLoggerOptions
    {
        public string TemplateFilePath { get; set; }
        public bool ShowLineNumbers { get; set; } = true;
        public bool ShowTimeStamp { get; set; } = true;
        public bool ShowClassName { get; set; } = true;
        public string ConsolePath { get; set; } = "/console";
        public string LogStreamUrl { get; set; } = "ws://localhost:5000/logstream";
        public bool ReverseOrder { get; set; } = true;
    }
}
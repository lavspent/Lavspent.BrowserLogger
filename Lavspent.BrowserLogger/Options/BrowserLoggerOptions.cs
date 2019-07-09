using Microsoft.Extensions.Logging.Console;

namespace Lavspent.BrowserLogger.Options
{
    public class BrowserLoggerOptions : ConsoleLoggerOptions
    {
        public BrowserLoggerOptions()
        {
            WebConsole = new WebConsoleOptions();
        }
        public WebConsoleOptions WebConsole { get; set; }
        public string TemplateFilePath { get; set; }
        public string ConsolePath { get; set; } = "/console";
    }
}
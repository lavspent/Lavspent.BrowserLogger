namespace Lavspent.BrowserLogger.Options
{
    public class WebConsoleOptions
    {
        public bool ShowLineNumbers { get; set; } = true;
        public bool ShowTimeStamp { get; set; } = true;
        public bool ShowClassName { get; set; } = true;
        public string DateFormatString { get; set; } = "HH:MM:ss.l";
        public string LogStreamUrl { get; set; } = "ws://localhost:5000/logstream";
        public bool NewOnTop { get; set; } = true;
    }
}
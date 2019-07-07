namespace Lavspent.BrowserLogger.Options
{
    internal class ScriptOptions
    {
        public ScriptOptions(BrowserLoggerOptions options)
        {
            WebSocketUrl = options.LogStreamUrl;
            ShowLineNumbers = options.ShowLineNumbers;
            ShowTimeStamp = options.ShowTimeStamp;
            ShowClassName = options.ShowClassName;
            ReverseOrder = options.ReverseOrder;
        }

        public bool ShowLineNumbers { get; set; }
        public bool ShowTimeStamp { get; set; }
        public bool ShowClassName { get; set; }
        public string WebSocketUrl { get; set; }
        public bool ReverseOrder { get; set; }
    }
}
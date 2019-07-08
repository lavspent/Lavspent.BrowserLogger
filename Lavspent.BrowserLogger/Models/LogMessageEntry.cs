using System;

namespace Lavspent.BrowserLogger.Models
{
    public struct LogMessageEntry
    {
        public string LogLevel { get; set; }
        public DateTime TimeStampUtc;
        public string Name;
        public string Message;
        public bool LogAsError;
    }
}
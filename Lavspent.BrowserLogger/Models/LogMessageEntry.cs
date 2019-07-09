using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lavspent.BrowserLogger.Models
{
    public struct LogMessageEntry
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public LogLevel LogLevel { get; set; }

        public DateTime TimeStampUtc { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
    }
}
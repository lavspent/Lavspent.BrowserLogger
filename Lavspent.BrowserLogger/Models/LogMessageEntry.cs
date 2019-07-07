// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
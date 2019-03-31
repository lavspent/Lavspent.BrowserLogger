// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Lavspent.BrowserLogger
{
    public struct LogMessageEntry
    {
        public string TimeStamp;
        public string LevelString;
        public BrowserColor LevelBackground;
        public BrowserColor LevelForeground;
        public BrowserColor MessageColor;
        public string Message;
        public bool LogAsError;
    }
}
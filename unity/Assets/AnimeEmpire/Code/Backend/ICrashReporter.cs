using System;
using System.Collections.Generic;

namespace AnimeEmpire.Backend
{
    public interface ICrashReporter
    {
        void ReportException(Exception ex, IReadOnlyDictionary<string, string> extra = null);
        void ReportError(string message, IReadOnlyDictionary<string, string> extra = null);
        void SetUserId(string userId);
        void SetTag(string key, string value);
    }
}

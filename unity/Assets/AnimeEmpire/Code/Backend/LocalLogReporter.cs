using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AnimeEmpire.Backend
{
    public class LocalLogReporter : ICrashReporter
    {
        readonly Dictionary<string, string> _tags = new();

        public void SetUserId(string userId) => SetTag("user_id", userId);
        public void SetTag(string key, string value) { if (!string.IsNullOrEmpty(key)) _tags[key] = value ?? ""; }

        public void ReportException(Exception ex, IReadOnlyDictionary<string, string> extra = null)
        {
            if (ex == null) return;
            Debug.LogError($"[Crash] {ex.GetType().Name}: {ex.Message}\n{FormatTags(extra)}\n{ex.StackTrace}");
        }

        public void ReportError(string message, IReadOnlyDictionary<string, string> extra = null)
        {
            Debug.LogError($"[Crash] {message}\n{FormatTags(extra)}");
        }

        string FormatTags(IReadOnlyDictionary<string, string> extra)
        {
            if (_tags.Count == 0 && (extra == null || extra.Count == 0)) return "tags: (none)";
            var sb = new StringBuilder("tags: ");
            foreach (var kv in _tags) sb.Append($"{kv.Key}={kv.Value} ");
            if (extra != null) foreach (var kv in extra) sb.Append($"{kv.Key}={kv.Value} ");
            return sb.ToString().TrimEnd();
        }
    }
}

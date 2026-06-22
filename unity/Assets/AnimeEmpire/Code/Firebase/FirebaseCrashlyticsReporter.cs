using System;
using System.Collections.Generic;
using AnimeEmpire.Backend;
using global::Firebase.Crashlytics;
using UnityEngine;

namespace AnimeEmpire.Firebase
{
    public class FirebaseCrashlyticsReporter : ICrashReporter
    {
        readonly Dictionary<string, string> _tags = new();

        public void ReportException(Exception ex, IReadOnlyDictionary<string, string> extra = null)
        {
            if (ex == null) return;
            ApplyTags(extra);
            Crashlytics.LogException(ex);
        }

        public void ReportError(string message, IReadOnlyDictionary<string, string> extra = null)
        {
            ApplyTags(extra);
            Crashlytics.Log(message ?? "");
        }

        public void SetUserId(string userId)
        {
            _tags["user_id"] = userId ?? "";
            Crashlytics.SetUserId(userId ?? "");
        }

        public void SetTag(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) return;
            _tags[key] = value ?? "";
            Crashlytics.SetCustomKey(key, value ?? "");
        }

        void ApplyTags(IReadOnlyDictionary<string, string> extra)
        {
            foreach (var kv in _tags) Crashlytics.SetCustomKey(kv.Key, kv.Value);
            if (extra != null) foreach (var kv in extra) Crashlytics.SetCustomKey(kv.Key, kv.Value ?? "");
        }
    }
}

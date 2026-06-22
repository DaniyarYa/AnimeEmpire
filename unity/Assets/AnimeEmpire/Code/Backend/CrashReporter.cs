using System;
using UnityEngine;

namespace AnimeEmpire.Backend
{
    /// Static facade. Phase 1 uses LocalLogReporter; Phase 2 swaps in
    /// FirebaseCrashlyticsReporter via SetProvider after Firebase init.
    public static class CrashReporter
    {
        public static ICrashReporter Current { get; private set; } = new LocalLogReporter();
        static bool _installed;

        public static void SetProvider(ICrashReporter reporter)
        {
            Current = reporter ?? new LocalLogReporter();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            if (_installed) return;
            _installed = true;
            Application.logMessageReceived += OnLogReceived;
        }

        static void OnLogReceived(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Exception && type != LogType.Error) return;
            // Avoid infinite loop: our own Debug.LogError lines start with "[Crash]" — skip those.
            if (!string.IsNullOrEmpty(condition) && condition.StartsWith("[Crash]")) return;
            try
            {
                Current?.ReportError(condition);
            }
            catch
            {
                // Reporter itself threw — drop silently to avoid cascading.
            }
        }
    }
}

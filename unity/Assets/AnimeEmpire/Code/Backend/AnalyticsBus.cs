using System;
using System.Collections.Generic;
using AnimeEmpire.Core;
using UnityEngine;

namespace AnimeEmpire.Backend
{
    public class AnalyticsBus : MonoBehaviour
    {
        public const float FlushEverySeconds = 30f;
        public const int FlushAtQueueSize = 50;

        public static AnalyticsBus Instance { get; private set; }
        public static IAnalyticsSink Sink { get; private set; } = new DebugLogSink();

        readonly List<Dictionary<string, object>> _queue = new();
        string _sessionId = "";
        float _timeSinceFlush;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            _sessionId = GenerateSessionId();
            LogEvent("lifecycle.session.started", null);
            Debug.Log($"[AnalyticsBus] ready, session={_sessionId}, sink={Sink.GetType().Name}");
        }

        void OnDestroy() { if (Instance == this) Instance = null; }

        void Update()
        {
            _timeSinceFlush += Time.unscaledDeltaTime;
            if (_timeSinceFlush >= FlushEverySeconds) Flush();
        }

        void OnApplicationPause(bool paused) { if (paused) { LogEvent("lifecycle.session.ended", null); Flush(); } }
        void OnApplicationQuit() { LogEvent("lifecycle.session.ended", null); Flush(); }

        /// Swap analytics destination. Phase 2 Firebase: AnalyticsBus.RegisterSink(new FirebaseAnalyticsSink()).
        public static void RegisterSink(IAnalyticsSink sink)
        {
            Sink = sink ?? new DebugLogSink();
            Debug.Log($"[AnalyticsBus] sink registered: {Sink.GetType().Name}");
        }

        public void LogEvent(string eventName, IDictionary<string, object> parameters)
        {
            var enriched = Enrich(eventName, parameters);
            _queue.Add(enriched);
            if (_queue.Count >= FlushAtQueueSize) Flush();
        }

        Dictionary<string, object> Enrich(string eventName, IDictionary<string, object> p)
        {
            var d = p != null ? new Dictionary<string, object>(p) : new Dictionary<string, object>();
            d["event"] = eventName;
            d["ts"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            d["session_id"] = _sessionId;
            d["app_version"] = Application.version;
            d["platform"] = Application.platform.ToString();
            d["locale"] = Localization.Instance != null ? Localization.Instance.Current : Localization.FallbackLocale;
            return d;
        }

        void Flush()
        {
            if (_queue.Count == 0) { _timeSinceFlush = 0f; return; }
            foreach (var ev in _queue)
            {
                if (!ev.TryGetValue("event", out var nObj)) continue;
                string name = nObj?.ToString() ?? "unknown";
                Sink?.Send(name, ev);
            }
            Sink?.Flush();
            _queue.Clear();
            _timeSinceFlush = 0f;
        }

        static string GenerateSessionId()
            => $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{UnityEngine.Random.Range(0, int.MaxValue)}";
    }
}

using System.Collections.Generic;

namespace AnimeEmpire.Backend
{
    /// Swap point for analytics destination. Default impl: Debug.Log batch flush
    /// (DebugLogSink). Phase 2: FirebaseAnalyticsSink, custom HTTP sender, etc.
    public interface IAnalyticsSink
    {
        void Send(string eventName, IReadOnlyDictionary<string, object> parameters);
        void Flush();
        void SetUserId(string userId);
    }

    public class DebugLogSink : IAnalyticsSink
    {
        public void Send(string eventName, IReadOnlyDictionary<string, object> parameters)
        {
            UnityEngine.Debug.Log($"[Analytics] {eventName} {(parameters != null ? parameters.Count : 0)} params");
        }
        public void Flush() { }
        public void SetUserId(string userId) => UnityEngine.Debug.Log($"[Analytics] user_id={userId}");
    }
}

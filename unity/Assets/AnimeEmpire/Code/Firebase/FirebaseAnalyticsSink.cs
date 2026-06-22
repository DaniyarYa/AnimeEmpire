using System.Collections.Generic;
using AnimeEmpire.Backend;
using global::Firebase.Analytics;

namespace AnimeEmpire.Firebase
{
    public class FirebaseAnalyticsSink : IAnalyticsSink
    {
        public void Send(string eventName, IReadOnlyDictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(eventName)) return;
            if (parameters == null || parameters.Count == 0)
            {
                FirebaseAnalytics.LogEvent(eventName);
                return;
            }
            var list = new List<Parameter>(parameters.Count);
            foreach (var kv in parameters)
            {
                if (string.IsNullOrEmpty(kv.Key) || kv.Value == null) continue;
                var p = BuildParam(kv.Key, kv.Value);
                if (p != null) list.Add(p);
            }
            FirebaseAnalytics.LogEvent(eventName, list.ToArray());
        }

        static Parameter BuildParam(string key, object value) => value switch
        {
            string s => new Parameter(key, s),
            long l => new Parameter(key, l),
            int i => new Parameter(key, i),
            double d => new Parameter(key, d),
            float f => new Parameter(key, f),
            bool b => new Parameter(key, b ? 1 : 0),
            _ => new Parameter(key, value.ToString()),
        };

        public void Flush() { /* Firebase auto-batches */ }
        public void SetUserId(string userId) => FirebaseAnalytics.SetUserId(userId ?? "");
    }
}

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AnimeEmpire.Core;
using AnimeEmpire.Data;
using AnimeEmpire.Platform;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace AnimeEmpire.Backend
{
    public class RemoteConfig : MonoBehaviour
    {
        public const string CacheFileName = "config_cache.json";
        public const int SchemaVersion = 1;

        public static RemoteConfig Instance { get; private set; }
        public static IRemoteConfigProvider OverrideProvider { get; private set; }

        IRemoteConfigProvider _builtin;

        public int Version => OverrideProvider?.Version ?? _builtin?.Version ?? 0;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            _builtin = new HttpRemoteConfigProvider();
            _ = _builtin.FetchAsync();
            Debug.Log($"[RemoteConfig] ready, version={Version}");
        }

        void OnDestroy() { if (Instance == this) Instance = null; }

        /// Register an alternative provider (e.g. Firebase). Falls back to built-in
        /// HTTP provider for keys it doesn't have.
        public static void RegisterProvider(IRemoteConfigProvider provider)
        {
            OverrideProvider = provider;
            Debug.Log($"[RemoteConfig] provider registered: {provider?.GetType().Name ?? "null"}");
        }

        public float GetFloat(string key, float def)
        {
            if (OverrideProvider != null)
            {
                float v = OverrideProvider.GetFloat(key, float.NaN);
                if (!float.IsNaN(v)) return v;
            }
            return _builtin != null ? _builtin.GetFloat(key, def) : def;
        }

        public int GetInt(string key, int def)
        {
            if (OverrideProvider != null)
            {
                int v = OverrideProvider.GetInt(key, int.MinValue);
                if (v != int.MinValue) return v;
            }
            return _builtin != null ? _builtin.GetInt(key, def) : def;
        }

        public bool GetBool(string key, bool def)
        {
            if (OverrideProvider != null)
            {
                // No null-safe bool sentinel; provider checks key existence internally.
                return OverrideProvider.GetBool(key, def);
            }
            return _builtin != null ? _builtin.GetBool(key, def) : def;
        }

        public string GetString(string key, string def)
        {
            if (OverrideProvider != null)
            {
                string v = OverrideProvider.GetString(key, null);
                if (!string.IsNullOrEmpty(v)) return v;
            }
            return _builtin != null ? _builtin.GetString(key, def) : def;
        }

        public Dictionary<string, object> ActiveVariants()
        {
            var d = new Dictionary<string, object>();
            if (_builtin != null) foreach (var kv in _builtin.ActiveVariants()) d[kv.Key] = kv.Value;
            if (OverrideProvider != null) foreach (var kv in OverrideProvider.ActiveVariants()) d[kv.Key] = kv.Value;
            return d;
        }

        public void ForceRefetch()
        {
            if (OverrideProvider != null) _ = OverrideProvider.FetchAsync();
            if (_builtin != null) _ = _builtin.FetchAsync();
        }
    }

    /// Default HTTP-based RemoteConfig provider. Reads URL from BackendConfig
    /// SO in Resources/. Persists disk cache. Falls back to built-in defaults.
    public class HttpRemoteConfigProvider : IRemoteConfigProvider
    {
        public const string CacheFileName = "config_cache.json";
        public int Version { get; private set; }

        Dictionary<string, object> _values = new();
        Dictionary<string, object> _abVariants = new();
        BackendConfig _config;

        string CachePath => Path.Combine(Application.persistentDataPath, CacheFileName);

        public HttpRemoteConfigProvider()
        {
            _config = Resources.Load<BackendConfig>("BackendConfig");
            LoadCacheOrDefaults();
        }

        public async Task<bool> FetchAsync()
        {
            if (_config == null || string.IsNullOrEmpty(_config.ConfigUrl))
            {
                Debug.Log("[HttpRemoteConfig] no URL configured");
                return false;
            }
            Debug.Log($"[HttpRemoteConfig] fetching {_config.ConfigUrl}");
            using var req = UnityWebRequest.Get(_config.ConfigUrl);
            await req.SendAsync(_config.RequestTimeoutSeconds);
            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[HttpRemoteConfig] fetch failed ({req.result}, http={req.responseCode}), using cache/defaults");
                return false;
            }
            var text = req.downloadHandler.text;
            try
            {
                var parsed = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
                if (parsed == null) { Debug.LogWarning("[HttpRemoteConfig] invalid JSON"); return false; }
                if (!Apply(parsed)) return false;
                SaveCache(text);
                Debug.Log($"[HttpRemoteConfig] fetched ok, version={Version}");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[HttpRemoteConfig] parse error: {e.Message}");
                return false;
            }
        }

        void LoadCacheOrDefaults()
        {
            if (File.Exists(CachePath))
            {
                try
                {
                    var raw = File.ReadAllText(CachePath);
                    var parsed = JsonConvert.DeserializeObject<Dictionary<string, object>>(raw);
                    if (parsed != null && Apply(parsed))
                    {
                        Debug.Log($"[HttpRemoteConfig] loaded from disk cache, version={Version}");
                        return;
                    }
                }
                catch { /* fall through */ }
            }
            ApplyDefaults();
        }

        bool Apply(Dictionary<string, object> data)
        {
            int incoming = data.TryGetValue("schema_version", out var sv) ? System.Convert.ToInt32(sv) : 0;
            if (incoming > RemoteConfig.SchemaVersion) return false;
            Version = data.TryGetValue("version", out var v) ? System.Convert.ToInt32(v) : 0;
            _values = data.TryGetValue("values", out var vals) && vals is Newtonsoft.Json.Linq.JObject jo
                ? jo.ToObject<Dictionary<string, object>>()
                : new Dictionary<string, object>();
            _abVariants = data.TryGetValue("ab_variants", out var ab) && ab is Newtonsoft.Json.Linq.JObject ja
                ? ja.ToObject<Dictionary<string, object>>()
                : new Dictionary<string, object>();
            EventBus.RaiseConfigLoaded(Version);
            return true;
        }

        void ApplyDefaults()
        {
            _values = new Dictionary<string, object>
            {
                ["economy.cost_growth_early"] = 1.12,
                ["economy.cost_growth_mid"] = 1.15,
                ["economy.cost_growth_late"] = 1.18,
                ["flags.enable_friends"] = false,
                ["flags.enable_battle_pass"] = false,
            };
            _abVariants = new Dictionary<string, object>();
            Version = 0;
            EventBus.RaiseConfigLoaded(0);
        }

        void SaveCache(string raw)
        {
            try { File.WriteAllText(CachePath, raw); }
            catch (System.Exception e) { Debug.LogWarning($"[HttpRemoteConfig] cache write failed: {e.Message}"); }
        }

        public float GetFloat(string key, float def) => _values.TryGetValue(key, out var v) ? System.Convert.ToSingle(v) : def;
        public int GetInt(string key, int def) => _values.TryGetValue(key, out var v) ? System.Convert.ToInt32(v) : def;
        public bool GetBool(string key, bool def) => _values.TryGetValue(key, out var v) ? System.Convert.ToBoolean(v) : def;
        public string GetString(string key, string def) => _values.TryGetValue(key, out var v) ? System.Convert.ToString(v) : def;
        public IReadOnlyDictionary<string, object> ActiveVariants() => _abVariants;
    }
}

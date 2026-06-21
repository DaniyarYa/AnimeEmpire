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

        public int Version { get; private set; }
        public int CurrentSchemaVersion { get; private set; } = SchemaVersion;

        Dictionary<string, object> _values = new();
        Dictionary<string, object> _abVariants = new();
        BackendConfig _config;

        string CachePath => Path.Combine(Application.persistentDataPath, CacheFileName);

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            _config = Resources.Load<BackendConfig>("BackendConfig");
            LoadCacheOrDefaults();
            if (_config != null && !string.IsNullOrEmpty(_config.ConfigUrl))
                _ = FetchRemoteAsync();
            else
                Debug.Log("[RemoteConfig] no URL configured");
            Debug.Log($"[RemoteConfig] ready, version={Version}");
        }

        void OnDestroy() { if (Instance == this) Instance = null; }

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
                        Debug.Log($"[RemoteConfig] loaded from disk cache, version={Version}");
                        return;
                    }
                }
                catch { /* fall through to defaults */ }
            }
            ApplyDefaults();
            Debug.Log("[RemoteConfig] using built-in defaults");
        }

        bool Apply(Dictionary<string, object> data)
        {
            int incoming = data.TryGetValue("schema_version", out var sv) ? System.Convert.ToInt32(sv) : 0;
            if (incoming > SchemaVersion)
            {
                Debug.LogWarning($"[RemoteConfig] schema_version={incoming} > supported={SchemaVersion}, ignoring");
                return false;
            }
            Version = data.TryGetValue("version", out var v) ? System.Convert.ToInt32(v) : 0;
            CurrentSchemaVersion = incoming;
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
            CurrentSchemaVersion = SchemaVersion;
            EventBus.RaiseConfigLoaded(0);
        }

        async Task FetchRemoteAsync()
        {
            Debug.Log($"[RemoteConfig] fetching {_config.ConfigUrl}");
            using var req = UnityWebRequest.Get(_config.ConfigUrl);
            await req.SendAsync(_config.RequestTimeoutSeconds);
            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[RemoteConfig] fetch failed ({req.result}, http={req.responseCode}), using cache/defaults");
                return;
            }
            var text = req.downloadHandler.text;
            try
            {
                var parsed = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
                if (parsed == null) { Debug.LogWarning("[RemoteConfig] invalid JSON"); return; }
                if (!Apply(parsed)) return;
                SaveCache(text);
                Debug.Log($"[RemoteConfig] fetched ok, version={Version}");
            }
            catch (System.Exception e) { Debug.LogWarning($"[RemoteConfig] parse error: {e.Message}"); }
        }

        void SaveCache(string raw)
        {
            try { File.WriteAllText(CachePath, raw); }
            catch (System.Exception e) { Debug.LogWarning($"[RemoteConfig] cache write failed: {e.Message}"); }
        }

        public float GetFloat(string key, float def) => _values.TryGetValue(key, out var v) ? System.Convert.ToSingle(v) : def;
        public int GetInt(string key, int def) => _values.TryGetValue(key, out var v) ? System.Convert.ToInt32(v) : def;
        public bool GetBool(string key, bool def) => _values.TryGetValue(key, out var v) ? System.Convert.ToBoolean(v) : def;
        public string GetString(string key, string def) => _values.TryGetValue(key, out var v) ? System.Convert.ToString(v) : def;

        public Dictionary<string, object> ActiveVariants() => new(_abVariants);

        public void ForceRefetch()
        {
            if (_config == null || string.IsNullOrEmpty(_config.ConfigUrl)) return;
            _ = FetchRemoteAsync();
        }
    }
}

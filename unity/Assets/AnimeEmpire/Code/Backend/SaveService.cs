using System;
using System.Collections.Generic;
using System.IO;
using AnimeEmpire.Core;
using Newtonsoft.Json;
using UnityEngine;

namespace AnimeEmpire.Backend
{
    public class SaveService : MonoBehaviour
    {
        public const string SaveFileName = "save_0.json";
        public const int SaveVersion = 1;
        public const float DebounceSeconds = 2f;

        public static SaveService Instance { get; private set; }

        Dictionary<string, object> _state;
        bool _pending;
        float _debounceTimer;
        ICloudSyncProvider _cloud = new LocalNoopProvider();
        string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        public void SetCloudProvider(ICloudSyncProvider provider) => _cloud = provider ?? new LocalNoopProvider();
        public ICloudSyncProvider CloudProvider => _cloud;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            EventBus.SaveDirty += OnSaveDirty;
            LoadState();
            if (GodotSaveMigrator.TryMigrate(_state))
            {
                SaveStateNow();
            }
            Debug.Log($"[SaveService] ready, save_version={(_state.TryGetValue("save_version", out var v) ? v : "new")}");
        }

        void OnDestroy()
        {
            EventBus.SaveDirty -= OnSaveDirty;
            if (Instance == this) Instance = null;
        }

        void OnSaveDirty()
        {
            _pending = true;
            _debounceTimer = DebounceSeconds;
        }

        void Update()
        {
            if (!_pending) return;
            _debounceTimer -= Time.unscaledDeltaTime;
            if (_debounceTimer <= 0f) Flush();
        }

        void OnApplicationPause(bool paused) { if (paused) Flush(); }
        void OnApplicationQuit() => Flush();

        public void Flush()
        {
            if (!_pending) return;
            SaveStateNow();
            _pending = false;
        }

        public void SaveStateNow()
        {
            _state["save_version"] = SaveVersion;
            _state["last_seen_at"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            try
            {
                var json = JsonConvert.SerializeObject(_state, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None,
                    Formatting = Formatting.None,
                });
                File.WriteAllText(SavePath, json);
                EventBus.RaiseSavePersisted();
                _ = TryUploadAsync();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SaveService] write failed: {e.Message}");
            }
        }

        async System.Threading.Tasks.Task TryUploadAsync()
        {
            if (_cloud == null || !_cloud.IsAuthenticated) return;
            try { await _cloud.UploadAsync(_state); }
            catch (Exception e) { Debug.LogWarning($"[SaveService] cloud upload failed: {e.Message}"); }
        }

        public void LoadState()
        {
            if (!File.Exists(SavePath)) { _state = NewState(); return; }
            try
            {
                var raw = File.ReadAllText(SavePath);
                var parsed = JsonConvert.DeserializeObject<Dictionary<string, object>>(raw);
                _state = parsed ?? NewState();
            }
            catch
            {
                Debug.LogWarning("[SaveService] corrupt save, starting fresh");
                _state = NewState();
            }
        }

        public Dictionary<string, object> GetState() => _state;

        public void PatchState(IDictionary<string, object> patch)
        {
            foreach (var kv in patch) _state[kv.Key] = kv.Value;
            EventBus.RaiseSaveDirty();
        }

        public static Dictionary<string, object> NewState()
        {
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return new Dictionary<string, object>
            {
                ["save_version"] = SaveVersion,
                ["created_at"] = now,
                ["last_seen_at"] = now,
                ["currencies"] = new Dictionary<string, object> { ["gold"] = 0, ["gems"] = 0 },
                ["buildings"] = new Dictionary<string, object>(),
                ["npcs"] = new List<object>(),
                ["prestige"] = new Dictionary<string, object>
                {
                    ["level"] = 0,
                    ["points"] = 0,
                    ["upgrades"] = new Dictionary<string, object>(),
                },
                ["tutorial"] = new Dictionary<string, object>
                {
                    ["step"] = 0,
                    ["completed"] = false,
                },
                ["settings"] = new Dictionary<string, object>
                {
                    ["sfx"] = 1.0,
                    ["music"] = 0.7,
                    ["locale"] = "en",
                },
            };
        }
    }
}

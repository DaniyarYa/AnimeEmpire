using System.Collections.Generic;
using AnimeEmpire.Core;
using AnimeEmpire.Data;
using UnityEngine;

namespace AnimeEmpire.Economy
{
    public class EconomySim : MonoBehaviour
    {
        public const float TickHz = 10f;
        public const float TickDt = 1f / TickHz;
        public const int OfflineCapSeconds = 4 * 3600;
        public const float OfflineEfficiency = 0.5f;

        public static EconomySim Instance { get; private set; }

        float _accumulator;
        readonly List<ProductionLine> _lines = new();
        readonly Dictionary<string, int> _inventory = new();
        int _gold;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            EventBus.ResourceProduced += OnResourceProduced;
            Debug.Log($"[EconomySim] ready, tick_hz={TickHz}");
        }

        void OnDestroy()
        {
            EventBus.ResourceProduced -= OnResourceProduced;
            if (Instance == this) Instance = null;
        }

        long _pauseTimestamp;
        void OnApplicationPause(bool paused)
        {
            if (paused) _pauseTimestamp = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            else if (_pauseTimestamp > 0)
            {
                long now = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                int elapsed = (int)(now - _pauseTimestamp);
                _pauseTimestamp = 0;
                if (elapsed > 0)
                {
                    var r = SimulateOffline(elapsed);
                    Debug.Log($"[EconomySim] resumed: offline {elapsed}s ({r.ElapsedEffective:0}s effective), produced {r.Resources.Count} resources");
                }
            }
        }

        void Update()
        {
            _accumulator += Time.deltaTime;
            while (_accumulator >= TickDt)
            {
                _accumulator -= TickDt;
                Tick(TickDt);
            }
        }

        void Tick(float dt)
        {
            for (int i = 0; i < _lines.Count; i++) _lines[i].Tick(dt, null);
        }

        public void RegisterLine(ProductionLine line)
        {
            if (line != null && !_lines.Contains(line)) _lines.Add(line);
        }

        public void UnregisterLine(ProductionLine line) => _lines.Remove(line);

        public ProductionLine RegisterLineFromDef(BuildingDef def, int level = 1)
        {
            var line = new ProductionLine { BuildingDef = def, CurrentLevel = level, Owned = true };
            RegisterLine(line);
            return line;
        }

        public int GetInventory(string resourceId)
            => _inventory.TryGetValue(resourceId, out var v) ? v : 0;

        public Dictionary<string, int> GetInventorySnapshot() => new(_inventory);

        public IReadOnlyList<ProductionLine> GetRegisteredLines() => _lines;

        public int GetGold() => _gold;

        public int SellInventory(ResourceDef resource)
        {
            if (resource == null) return 0;
            int amount = GetInventory(resource.Id);
            if (amount == 0) return 0;
            int gold = amount * resource.BaseSellPrice;
            _inventory[resource.Id] = 0;
            _gold += gold;
            EventBus.RaiseResourceSold(resource.Id, amount, gold);
            EventBus.RaiseCurrencyChanged("gold", _gold);
            EventBus.RaiseSaveDirty();
            return gold;
        }

        public bool SpendGold(int amount)
        {
            if (amount < 0 || _gold < amount) return false;
            _gold -= amount;
            EventBus.RaiseCurrencyChanged("gold", _gold);
            EventBus.RaiseSaveDirty();
            return true;
        }

        public void GrantGold(int amount)
        {
            if (amount <= 0) return;
            _gold += amount;
            EventBus.RaiseCurrencyChanged("gold", _gold);
            EventBus.RaiseSaveDirty();
        }

        void OnResourceProduced(string buildingId, string resourceId, int amount)
        {
            _inventory.TryGetValue(resourceId, out var current);
            _inventory[resourceId] = current + amount;
        }

        public OfflineResult SimulateOffline(int elapsedSeconds)
        {
            int capped = Mathf.Max(0, Mathf.Min(elapsedSeconds, OfflineCapSeconds));
            float effective = capped * OfflineEfficiency;
            var produced = new Dictionary<string, int>();

            if (capped <= 0 || _lines.Count == 0)
                return new OfflineResult { Gold = 0, Resources = produced, ElapsedEffective = effective };

            for (int i = 0; i < _lines.Count; i++)
            {
                var line = _lines[i];
                if (line == null || !line.Owned || line.BuildingDef == null || line.BuildingDef.OutputResource == null) continue;
                float cycle = line.BuildingDef.BaseCycleSeconds * Mathf.Pow(ProductionLine.CycleDecayPerLevel, line.CurrentLevel - 1);
                if (cycle < 0.0001f) continue;
                int cycles = Mathf.FloorToInt(effective / cycle);
                if (cycles <= 0) continue;
                int amount = cycles * line.BuildingDef.OutputAmount;
                string resId = line.BuildingDef.OutputResource.Id;
                produced.TryGetValue(resId, out var prev);
                produced[resId] = prev + amount;
            }

            // OnResourceProduced handler increments _inventory — emit once per batched resource.
            foreach (var kv in produced)
                EventBus.RaiseResourceProduced("offline", kv.Key, kv.Value);
            if (produced.Count > 0) EventBus.RaiseSaveDirty();

            return new OfflineResult { Gold = 0, Resources = produced, ElapsedEffective = effective };
        }

        public struct OfflineResult
        {
            public int Gold;
            public Dictionary<string, int> Resources;
            public float ElapsedEffective;
        }
    }
}

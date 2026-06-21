using System.Collections.Generic;
using AnimeEmpire.Entities;
using UnityEngine;

namespace AnimeEmpire.Backend
{
    public class NPCSystem : MonoBehaviour
    {
        public static NPCSystem Instance { get; private set; }

        readonly Dictionary<string, NPC> _npcs = new();

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            Debug.Log("[NPCSystem] ready");
        }

        void OnDestroy() { if (Instance == this) Instance = null; }

        public void Register(string instanceId, NPC npc) => _npcs[instanceId] = npc;
        public void Unregister(string instanceId) => _npcs.Remove(instanceId);
        public NPC Get(string instanceId) => _npcs.TryGetValue(instanceId, out var n) ? n : null;
        public int Count => _npcs.Count;

        public void Tick(float dt) { /* P1-024 dispatch TBD */ }
    }
}

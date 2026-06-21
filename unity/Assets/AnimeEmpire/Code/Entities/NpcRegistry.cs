using System.Collections.Generic;
using UnityEngine;

namespace AnimeEmpire.Entities
{
    public static class NpcRegistry
    {
        static readonly List<NPC> _all = new();
        public static IReadOnlyList<NPC> All => _all;

        public static void Register(NPC n) { if (n != null && !_all.Contains(n)) _all.Add(n); }
        public static void Unregister(NPC n) => _all.Remove(n);

        public static NPC FindAvailableForCategory(string buildingCategory)
        {
            for (int i = 0; i < _all.Count; i++)
            {
                var n = _all[i];
                if (n == null || !n.IsAvailable()) continue;
                if (n.Def == null) continue;
                var attached = n.Def.AttachedBuildingCategory;
                if (string.IsNullOrEmpty(attached) || attached == buildingCategory) return n;
            }
            return null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() => _all.Clear();
    }
}

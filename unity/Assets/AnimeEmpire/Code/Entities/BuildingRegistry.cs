using System.Collections.Generic;
using UnityEngine;

namespace AnimeEmpire.Entities
{
    public static class BuildingRegistry
    {
        static readonly List<Building> _all = new();

        public static IReadOnlyList<Building> All => _all;

        public static void Register(Building b)
        {
            if (b != null && !_all.Contains(b)) _all.Add(b);
        }

        public static void Unregister(Building b) => _all.Remove(b);

        public static Building FindFirstByCategory(string category)
        {
            for (int i = 0; i < _all.Count; i++)
            {
                var b = _all[i];
                if (b != null && b.Def != null && b.Def.Category == category) return b;
            }
            return null;
        }

        public static Building FindById(string id)
        {
            for (int i = 0; i < _all.Count; i++)
            {
                var b = _all[i];
                if (b != null && b.Def != null && b.Def.Id == id) return b;
            }
            return null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() => _all.Clear();
    }
}

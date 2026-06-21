using System;
using UnityEngine;

namespace AnimeEmpire.Core
{
    public static class EventBus
    {
        public static event Action<string, string, int> ResourceProduced;
        public static event Action<string, int, int> ResourceSold;
        public static event Action<string, int> BuildingUpgraded;
        public static event Action<string, string> NpcHired;
        public static event Action<string, string> NpcTaskCompleted;
        public static event Action SaveDirty;
        public static event Action SavePersisted;
        public static event Action<string, int> CurrencyChanged;
        public static event Action<string, bool> IapCompleted;
        public static event Action<int> PrestigeTriggered;
        public static event Action<int> ConfigLoaded;
        public static event Action<string, string> ScreenChanged;

        public static void RaiseResourceProduced(string buildingId, string resourceId, int amount)
            => ResourceProduced?.Invoke(buildingId, resourceId, amount);

        public static void RaiseResourceSold(string resourceId, int amount, int gold)
            => ResourceSold?.Invoke(resourceId, amount, gold);

        public static void RaiseBuildingUpgraded(string buildingId, int newLevel)
            => BuildingUpgraded?.Invoke(buildingId, newLevel);

        public static void RaiseNpcHired(string npcId, string instanceId)
            => NpcHired?.Invoke(npcId, instanceId);

        public static void RaiseNpcTaskCompleted(string instanceId, string task)
            => NpcTaskCompleted?.Invoke(instanceId, task);

        public static void RaiseSaveDirty() => SaveDirty?.Invoke();
        public static void RaiseSavePersisted() => SavePersisted?.Invoke();

        public static void RaiseCurrencyChanged(string kind, int value)
            => CurrencyChanged?.Invoke(kind, value);

        public static void RaiseIapCompleted(string sku, bool success)
            => IapCompleted?.Invoke(sku, success);

        public static void RaisePrestigeTriggered(int pointsEarned)
            => PrestigeTriggered?.Invoke(pointsEarned);

        public static void RaiseConfigLoaded(int version)
            => ConfigLoaded?.Invoke(version);

        public static void RaiseScreenChanged(string from, string to)
            => ScreenChanged?.Invoke(from, to);

        // Static delegate state persists across PlayMode/test runs when domain reload is disabled,
        // causing stale handlers from previous play sessions. SubsystemRegistration runs before
        // any Awake on entering Play mode, clearing the slate.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset()
        {
            ResourceProduced = null;
            ResourceSold = null;
            BuildingUpgraded = null;
            NpcHired = null;
            NpcTaskCompleted = null;
            SaveDirty = null;
            SavePersisted = null;
            CurrencyChanged = null;
            IapCompleted = null;
            PrestigeTriggered = null;
            ConfigLoaded = null;
            ScreenChanged = null;
        }
    }
}

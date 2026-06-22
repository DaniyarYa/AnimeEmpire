using System.Collections.Generic;
using AnimeEmpire.Economy;
using UnityEngine;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

namespace AnimeEmpire.Backend
{
    public class NotificationService : MonoBehaviour
    {
        public const string ChannelId = "anime_empire_default";
        public const string ChannelName = "Anime Empire";
        public const int MinDelaySeconds = 60;
        public const int MaxDelaySeconds = 24 * 3600;

        public static NotificationService Instance { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            RegisterChannel();
            RequestAuthorization();
            Debug.Log("[NotificationService] ready");
        }

        void OnDestroy() { if (Instance == this) Instance = null; }

        void OnApplicationPause(bool paused)
        {
            if (paused) ScheduleNextHarvestReady();
            else CancelAll();
        }

        void OnApplicationQuit() => ScheduleNextHarvestReady();

        public void ScheduleNextHarvestReady()
        {
            var sim = EconomySim.Instance;
            int seconds = ComputeNextHarvestSeconds(sim != null ? sim.GetRegisteredLines() : null);
            if (seconds < MinDelaySeconds) return;
            if (seconds > MaxDelaySeconds) seconds = MaxDelaySeconds;
            Schedule("harvest_ready", "Anime Empire", "Your production is ready to collect", seconds);
        }

        public static int ComputeNextHarvestSeconds(IReadOnlyList<ProductionLine> lines)
        {
            if (lines == null || lines.Count == 0) return 0;
            float best = float.PositiveInfinity;
            for (int i = 0; i < lines.Count; i++)
            {
                var l = lines[i];
                if (l == null || !l.Owned || l.BuildingDef == null) continue;
                float cycle = l.BuildingDef.BaseCycleSeconds * Mathf.Pow(ProductionLine.CycleDecayPerLevel, l.CurrentLevel - 1);
                float remaining = (1f - l.GetProgress()) * cycle;
                if (remaining < best) best = remaining;
            }
            if (float.IsInfinity(best) || best <= 0f) return 0;
            return Mathf.CeilToInt(best);
        }

        void Schedule(string id, string title, string body, int delaySeconds)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var n = new AndroidNotification
            {
                Title = title,
                Text = body,
                FireTime = System.DateTime.Now.AddSeconds(delaySeconds),
                SmallIcon = "icon_0",
                LargeIcon = "icon_1",
            };
            AndroidNotificationCenter.SendNotification(n, ChannelId);
#elif UNITY_IOS && !UNITY_EDITOR
            var n = new iOSNotification
            {
                Identifier = id,
                Title = title,
                Body = body,
                ShowInForeground = false,
                Trigger = new iOSNotificationTimeIntervalTrigger
                {
                    TimeInterval = System.TimeSpan.FromSeconds(delaySeconds),
                    Repeats = false,
                },
            };
            iOSNotificationCenter.ScheduleNotification(n);
#else
            Debug.Log($"[NotificationService] (editor stub) schedule '{id}' in {delaySeconds}s");
#endif
        }

        void CancelAll()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidNotificationCenter.CancelAllScheduledNotifications();
#elif UNITY_IOS && !UNITY_EDITOR
            iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif
        }

        void RegisterChannel()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var ch = new AndroidNotificationChannel
            {
                Id = ChannelId,
                Name = ChannelName,
                Importance = Importance.Default,
                Description = "Production-ready and event reminders.",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(ch);
#endif
        }

        void RequestAuthorization()
        {
#if UNITY_IOS && !UNITY_EDITOR
            var options = AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound;
            using var req = new AuthorizationRequest(options, true);
            // Fire and forget — user gets prompt; we read req.Granted in Phase 3 onboarding flow.
#endif
        }
    }
}

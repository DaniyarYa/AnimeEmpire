using System;
using UnityEngine;

namespace AnimeEmpire.Core
{
    public class GameState : MonoBehaviour
    {
        public const string ScreenBoot = "boot";
        public const string ScreenWorld = "world";

        public static GameState Instance { get; private set; }

        public string CurrentScreen = ScreenBoot;
        public long SessionStartTime;
        public bool IsTutorialActive;
        public int CurrentPrestigeLevel;
        public string CurrentRegion = "village";
        public string SessionId = "";
        public string UserId = "";
        public string CountryCode = "";

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            SessionStartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Debug.Log($"[GameState] ready, session_start={SessionStartTime}");
        }

        void OnDestroy() { if (Instance == this) Instance = null; }
    }
}

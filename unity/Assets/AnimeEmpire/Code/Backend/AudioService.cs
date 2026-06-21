using System.Collections.Generic;
using AnimeEmpire.Core;
using UnityEngine;

namespace AnimeEmpire.Backend
{
    /// Plays SFX in response to gameplay events. Phase 1: stub-safe (no clips
    /// authored). Clip refs assignable via Inspector on Bootstrap prefab; missing
    /// clips silently skipped.
    public class AudioService : MonoBehaviour
    {
        public static AudioService Instance { get; private set; }

        [System.Serializable]
        public class ClipEntry { public string Key; public AudioClip Clip; }

        [SerializeField] List<ClipEntry> _clips = new();
        [Range(0f, 1f)] public float SfxVolume = 1f;
        [Range(0f, 1f)] public float MusicVolume = 0.7f;

        AudioSource _sfxSource;
        Dictionary<string, AudioClip> _clipMap;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;
            _sfxSource.loop = false;
            _sfxSource.spatialBlend = 0f;
            _clipMap = new Dictionary<string, AudioClip>();
            foreach (var e in _clips) if (!string.IsNullOrEmpty(e.Key) && e.Clip != null) _clipMap[e.Key] = e.Clip;
            EventBus.ResourceProduced += OnResourceProduced;
            EventBus.ResourceSold += OnResourceSold;
            EventBus.BuildingUpgraded += OnBuildingUpgraded;
            EventBus.IapCompleted += OnIapCompleted;
            Debug.Log($"[AudioService] ready, {_clipMap.Count} clips registered");
        }

        void OnDestroy()
        {
            EventBus.ResourceProduced -= OnResourceProduced;
            EventBus.ResourceSold -= OnResourceSold;
            EventBus.BuildingUpgraded -= OnBuildingUpgraded;
            EventBus.IapCompleted -= OnIapCompleted;
            if (Instance == this) Instance = null;
        }

        public void Play(string key)
        {
            if (_clipMap == null) return;
            if (!_clipMap.TryGetValue(key, out var clip) || clip == null) return;
            _sfxSource.PlayOneShot(clip, SfxVolume);
        }

        void OnResourceProduced(string _, string __, int ___) => Play("sfx.produce");
        void OnResourceSold(string _, int __, int ___) => Play("sfx.sell");
        void OnBuildingUpgraded(string _, int __) => Play("sfx.upgrade");
        void OnIapCompleted(string _, bool ok) => Play(ok ? "sfx.purchase_ok" : "sfx.purchase_fail");
    }
}

using System;
using System.Collections.Generic;
using AnimeEmpire.Backend;
using UnityEngine;

namespace AnimeEmpire.Core
{
    /// Persists player settings via SaveService["settings"]. Exposes typed properties
    /// + SettingsChanged event so AudioService / Localization can react.
    public class SettingsService : MonoBehaviour
    {
        public const string SettingsKey = "settings";
        public const string SfxKey = "sfx";
        public const string MusicKey = "music";
        public const string LocaleKey = "locale";

        public static SettingsService Instance { get; private set; }

        public event Action SettingsChanged;

        float _sfx = 1f;
        float _music = 0.7f;
        string _locale = Localization.FallbackLocale;
        bool _loaded;

        public float Sfx { get => _sfx; set => SetFloat(SfxKey, ref _sfx, value); }
        public float Music { get => _music; set => SetFloat(MusicKey, ref _music, value); }
        public string Locale { get => _locale; set => SetLocale(value); }

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            LoadFromSave();
            Debug.Log($"[SettingsService] ready sfx={_sfx:0.0} music={_music:0.0} locale={_locale}");
        }

        void OnDestroy() { if (Instance == this) Instance = null; }

        public void LoadFromSave()
        {
            var save = SaveService.Instance;
            if (save == null) return;
            var state = save.GetState();
            if (!state.TryGetValue(SettingsKey, out var raw) || raw is not Dictionary<string, object> dict) return;
            if (dict.TryGetValue(SfxKey, out var sfx)) _sfx = Mathf.Clamp01(Convert.ToSingle(sfx));
            if (dict.TryGetValue(MusicKey, out var music)) _music = Mathf.Clamp01(Convert.ToSingle(music));
            if (dict.TryGetValue(LocaleKey, out var locale)) _locale = locale?.ToString() ?? Localization.FallbackLocale;
            _loaded = true;
            ApplyToServices();
            SettingsChanged?.Invoke();
        }

        void SetFloat(string key, ref float field, float value)
        {
            value = Mathf.Clamp01(value);
            if (Mathf.Approximately(field, value)) return;
            field = value;
            WriteBack(key, value);
            ApplyToServices();
            SettingsChanged?.Invoke();
        }

        void SetLocale(string code)
        {
            if (code == _locale) return;
            _locale = code;
            WriteBack(LocaleKey, code);
            if (Localization.Instance != null) Localization.Instance.SetLocale(code);
            SettingsChanged?.Invoke();
        }

        void WriteBack(string key, object value)
        {
            var save = SaveService.Instance;
            if (save == null) return;
            var state = save.GetState();
            if (!state.TryGetValue(SettingsKey, out var raw) || raw is not Dictionary<string, object> dict)
            {
                dict = new Dictionary<string, object>();
                state[SettingsKey] = dict;
            }
            dict[key] = value;
            EventBus.RaiseSaveDirty();
        }

        void ApplyToServices()
        {
            var audio = AudioService.Instance;
            if (audio != null)
            {
                audio.SfxVolume = _sfx;
                audio.MusicVolume = _music;
            }
        }
    }
}

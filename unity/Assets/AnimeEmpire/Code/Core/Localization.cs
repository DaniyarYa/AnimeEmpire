using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace AnimeEmpire.Core
{
    public class Localization : MonoBehaviour
    {
        public const string FallbackLocale = "en";
        public static readonly string[] SupportedLocales = { "en", "es", "fr", "de", "ja" };
        public const string DefaultTable = "UI";

        public static Localization Instance { get; private set; }
        public string Current { get; private set; } = FallbackLocale;

        bool _ready;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DetectInitialLocale();
            Debug.Log($"[Localization] ready, locale={Current}");
        }

        void OnDestroy() { if (Instance == this) Instance = null; }

        void DetectInitialLocale()
        {
            var sys = Application.systemLanguage;
            string code = MapSystemLanguageToCode(sys);
            SetLocale(code);
        }

        static string MapSystemLanguageToCode(SystemLanguage lang) => lang switch
        {
            SystemLanguage.English => "en",
            SystemLanguage.Spanish => "es",
            SystemLanguage.French => "fr",
            SystemLanguage.German => "de",
            SystemLanguage.Japanese => "ja",
            _ => FallbackLocale,
        };

        public void SetLocale(string code)
        {
            bool supported = false;
            foreach (var s in SupportedLocales) if (s == code) { supported = true; break; }
            if (!supported) code = FallbackLocale;
            Current = code;
            // If Localization package is ready, apply selected locale.
            if (LocalizationSettings.HasSettings)
            {
                var locale = LocalizationSettings.AvailableLocales?.GetLocale(code);
                if (locale != null) LocalizationSettings.SelectedLocale = locale;
            }
        }

        public static string T(string key) => T(DefaultTable, key);

        public static string T(string table, string key)
        {
            if (string.IsNullOrEmpty(key)) return key;
            if (!LocalizationSettings.HasSettings) return key;
            try
            {
                var s = LocalizationSettings.StringDatabase.GetLocalizedString(table, key);
                return string.IsNullOrEmpty(s) ? key : s;
            }
            catch
            {
                return key;
            }
        }

        public static string TFormat(string key, params object[] args)
        {
            var s = T(key);
            return args == null || args.Length == 0 ? s : string.Format(s, args);
        }
    }
}

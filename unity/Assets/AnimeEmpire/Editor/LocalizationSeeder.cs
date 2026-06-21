using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace AnimeEmpire.Editor
{
    public static class LocalizationSeeder
    {
        const string LocalesFolder = "Assets/AnimeEmpire/Localization/Locales";
        const string TablesFolder = "Assets/AnimeEmpire/Localization/StringTables";
        const string SettingsFolder = "Assets/AnimeEmpire/Localization";
        const string TableName = "UI";

        // Phase 1 ships RU + EN. ES/FR/DE/JA placeholders added but empty.
        static readonly (string code, SystemLanguage lang, string english)[] LocaleSpecs =
        {
            ("en", SystemLanguage.English,  "English"),
            ("ru", SystemLanguage.Russian,  "Russian"),
            ("es", SystemLanguage.Spanish,  "Spanish"),
            ("fr", SystemLanguage.French,   "French"),
            ("de", SystemLanguage.German,   "German"),
            ("ja", SystemLanguage.Japanese, "Japanese"),
        };

        static readonly Dictionary<string, (string en, string ru)> Entries = new()
        {
            { "building.wheat_farm.name", ("Wheat Farm",  "Пшеничная ферма") },
            { "building.mill.name",       ("Mill",        "Мельница") },
            { "building.bakery.name",     ("Bakery",      "Пекарня") },
            { "building.market.name",     ("Market",      "Рынок") },
            { "resource.wheat.name",      ("Wheat",       "Пшеница") },
            { "resource.flour.name",      ("Flour",       "Мука") },
            { "resource.bread.name",      ("Bread",       "Хлеб") },
            { "npc.gatherer_farmer.name", ("Farmer",      "Фермер") },
            { "ui.start_production",      ("Start production", "Начать производство") },
            { "ui.producing",             ("Producing...", "Производство...") },
            { "ui.worker_assigned",       ("Worker assigned", "Рабочий назначен") },
            { "ui.assign",                ("Assign", "Назначить") },
            { "ui.dismiss",               ("Dismiss", "Уволить") },
            { "ui.close",                 ("Close", "Закрыть") },
            { "ui.sell_all",              ("Sell all", "Продать всё") },
            { "ui.loading",               ("Loading...", "Загрузка...") },
            { "ui.entering_world",        ("Entering world...", "Запуск мира...") },
        };

        [MenuItem("Tools/Anime Empire/Seed Localization (UI table)")]
        public static void Seed()
        {
            EnsureFolder(LocalesFolder);
            EnsureFolder(TablesFolder);

            var settings = EnsureSettings();
            var localesByCode = EnsureLocales(settings);

            var collection = FindCollection(TableName) ?? LocalizationEditorSettings.CreateStringTableCollection(TableName, TablesFolder);
            foreach (var locale in localesByCode.Values)
            {
                bool hasTable = false;
                foreach (var t in collection.StringTables)
                {
                    if (t != null && t.LocaleIdentifier == locale.Identifier) { hasTable = true; break; }
                }
                if (!hasTable) collection.AddNewTable(locale.Identifier);
            }

            foreach (var (key, vals) in Entries)
            {
                if (collection.SharedData.GetEntry(key) == null) collection.SharedData.AddKey(key);
                SetEntry(collection, localesByCode["en"], key, vals.en);
                SetEntry(collection, localesByCode["ru"], key, vals.ru);
            }

            EditorUtility.SetDirty(collection.SharedData);
            foreach (var t in collection.StringTables) if (t != null) EditorUtility.SetDirty(t);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[LocalizationSeeder] Seeded {Entries.Count} keys × en+ru into table '{TableName}'.");
        }

        static LocalizationSettings EnsureSettings()
        {
            var settings = LocalizationEditorSettings.ActiveLocalizationSettings;
            if (settings != null) return settings;
            settings = ScriptableObject.CreateInstance<LocalizationSettings>();
            AssetDatabase.CreateAsset(settings, $"{SettingsFolder}/LocalizationSettings.asset");
            LocalizationEditorSettings.ActiveLocalizationSettings = settings;
            return settings;
        }

        static Dictionary<string, Locale> EnsureLocales(LocalizationSettings settings)
        {
            var map = new Dictionary<string, Locale>();
            foreach (var (code, lang, english) in LocaleSpecs)
            {
                var path = $"{LocalesFolder}/{code}.asset";
                var existing = AssetDatabase.LoadAssetAtPath<Locale>(path);
                if (existing == null)
                {
                    var locale = Locale.CreateLocale(lang);
                    locale.name = $"{english} ({code})";
                    AssetDatabase.CreateAsset(locale, path);
                    LocalizationEditorSettings.AddLocale(locale);
                    existing = locale;
                }
                map[code] = existing;
            }
            return map;
        }

        static StringTableCollection FindCollection(string name)
        {
            foreach (var c in LocalizationEditorSettings.GetStringTableCollections())
                if (c.TableCollectionName == name) return c;
            return null;
        }

        static void SetEntry(StringTableCollection collection, Locale locale, string key, string value)
        {
            foreach (var t in collection.StringTables)
            {
                if (t == null || t.LocaleIdentifier != locale.Identifier) continue;
                var entry = t.GetEntry(key) ?? t.AddEntry(key, value);
                entry.Value = value;
                return;
            }
        }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parts = path.Split('/');
            var cur = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                var next = cur + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(cur, parts[i]);
                cur = next;
            }
        }
    }
}

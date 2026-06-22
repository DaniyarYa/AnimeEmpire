using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace AnimeEmpire.Editor
{
    public static class FirebaseDefineToggle
    {
        const string Define = "FIREBASE_ENABLED";

        static readonly NamedBuildTarget[] Targets =
        {
            NamedBuildTarget.Android,
            NamedBuildTarget.iOS,
            NamedBuildTarget.Standalone,
        };

        [MenuItem("Tools/Anime Empire/Firebase/Enable FIREBASE_ENABLED define")]
        public static void Enable() => SetDefine(true);

        [MenuItem("Tools/Anime Empire/Firebase/Disable FIREBASE_ENABLED define")]
        public static void Disable() => SetDefine(false);

        [MenuItem("Tools/Anime Empire/Firebase/Verify SDK + config")]
        public static void Verify()
        {
            var problems = new List<string>();

            // 1. Define present?
            var current = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Android);
            if (!current.Contains(Define))
                problems.Add("FIREBASE_ENABLED define missing for Android. Run Enable menu.");

            // 2. Platform config files?
            var streaming = "Assets/StreamingAssets";
            if (!File.Exists($"{streaming}/google-services.json"))
                problems.Add("google-services.json missing in StreamingAssets/. Download from Firebase Console.");
            if (!File.Exists($"{streaming}/GoogleService-Info.plist"))
                problems.Add("GoogleService-Info.plist missing in StreamingAssets/. Needed for iOS build.");

            // 3. SDK assemblies present?
            foreach (var name in new[] { "Firebase.App", "Firebase.Auth", "Firebase.Analytics", "Firebase.Crashlytics", "Firebase.RemoteConfig", "Firebase.Messaging" })
            {
                if (System.AppDomain.CurrentDomain.GetAssemblies()
                        .FindIndex(a => a.GetName().Name == name) < 0)
                    problems.Add($"{name}.dll not loaded. Reimport {name.Replace("Firebase.", "Firebase")}.unitypackage.");
            }

            if (problems.Count == 0)
            {
                Debug.Log("[Firebase] verify ok — define + configs + SDK all present.");
                return;
            }
            Debug.LogError($"[Firebase] {problems.Count} issue(s):\n  - {string.Join("\n  - ", problems)}");
        }

        static void SetDefine(bool enabled)
        {
            foreach (var target in Targets)
            {
                var current = PlayerSettings.GetScriptingDefineSymbols(target);
                var parts = new HashSet<string>(current.Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));
                if (enabled) parts.Add(Define);
                else parts.Remove(Define);
                var next = string.Join(";", parts);
                if (next == current) continue;
                PlayerSettings.SetScriptingDefineSymbols(target, next);
                Debug.Log($"[Firebase] {target.TargetName}: {(enabled ? "+" : "-")}{Define}");
            }
            AssetDatabase.SaveAssets();
        }
    }

    static class ListExt
    {
        public static int FindIndex<T>(this T[] arr, System.Predicate<T> pred)
        {
            for (int i = 0; i < arr.Length; i++) if (pred(arr[i])) return i;
            return -1;
        }
    }
}

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace AnimeEmpire.Backend
{
    /// Phase 2 hook: read Godot's user://save_0.bin (JSON despite ".bin" extension) and
    /// merge currencies/prestige/tutorial into Unity save schema once. Sets
    /// _state["migrated_from_godot"] = true so re-runs no-op.
    public static class GodotSaveMigrator
    {
        public const string GodotSaveFileName = "save_0.bin";
        public const string MigratedFlag = "migrated_from_godot";

        /// Tries to find Godot's save file alongside Unity's persistentDataPath.
        /// On mobile, both engines write to ~/Library/Application Support/com.animeempire.app/
        /// (iOS) or /data/data/com.animeempire.app/files/ (Android) — so save_0.bin may
        /// sit next to our save_0.json after a Godot install upgrade.
        public static string FindGodotSavePath()
        {
            var candidates = new[]
            {
                Path.Combine(Application.persistentDataPath, GodotSaveFileName),
                Path.Combine(Application.persistentDataPath, "..", GodotSaveFileName),
            };
            foreach (var c in candidates) if (File.Exists(c)) return c;
            return null;
        }

        /// Returns true if migration happened (state was modified).
        public static bool TryMigrate(IDictionary<string, object> unityState)
        {
            if (unityState == null) return false;
            if (unityState.TryGetValue(MigratedFlag, out var f) && f is bool b && b) return false;

            var path = FindGodotSavePath();
            if (string.IsNullOrEmpty(path)) return false;

            Dictionary<string, object> godotState;
            try
            {
                var raw = File.ReadAllText(path);
                godotState = JsonConvert.DeserializeObject<Dictionary<string, object>>(raw);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[GodotSaveMigrator] read failed: {e.Message}");
                return false;
            }
            if (godotState == null) return false;

            CopyIfPresent(godotState, unityState, "currencies");
            CopyIfPresent(godotState, unityState, "prestige");
            CopyIfPresent(godotState, unityState, "tutorial");
            CopyIfPresent(godotState, unityState, "buildings");
            CopyIfPresent(godotState, unityState, "npcs");
            CopyIfPresent(godotState, unityState, "settings");

            unityState[MigratedFlag] = true;
            Debug.Log($"[GodotSaveMigrator] migrated from {path}");
            return true;
        }

        static void CopyIfPresent(IDictionary<string, object> src, IDictionary<string, object> dst, string key)
        {
            if (src.TryGetValue(key, out var v) && v != null) dst[key] = v;
        }
    }
}

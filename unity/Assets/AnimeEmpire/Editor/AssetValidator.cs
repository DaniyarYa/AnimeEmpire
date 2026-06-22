using System.Collections.Generic;
using System.Text;
using AnimeEmpire.Data;
using UnityEditor;
using UnityEngine;

namespace AnimeEmpire.Editor
{
    public static class AssetValidator
    {
        [MenuItem("Tools/Anime Empire/Validate Assets")]
        public static void ValidateAll()
        {
            var errors = new List<string>();
            ValidateType<BuildingDef>(errors, ValidateBuilding);
            ValidateType<ResourceDef>(errors, ValidateResource);
            ValidateType<NPCDef>(errors, ValidateNpc);
            ValidateType<IapProductDef>(errors, ValidateIap);
            ValidateType<TutorialStep>(errors, ValidateTutorialStep);
            CheckUniqueIds<BuildingDef>(errors);
            CheckUniqueIds<ResourceDef>(errors);
            CheckUniqueIds<NPCDef>(errors);
            CheckUniqueIds<IapProductDef>(errors);
            CheckUniqueIds<TutorialStep>(errors);

            if (errors.Count == 0)
            {
                Debug.Log("[AssetValidator] all assets pass.");
                return;
            }
            var sb = new StringBuilder($"[AssetValidator] {errors.Count} issues:\n");
            foreach (var e in errors) sb.AppendLine("  - " + e);
            Debug.LogError(sb.ToString());
        }

        static void ValidateType<T>(List<string> errors, System.Action<T, string, List<string>> rule) where T : ScriptableObject
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset == null) continue;
                rule(asset, path, errors);
            }
        }

        static void CheckUniqueIds<T>(List<string> errors) where T : GameResourceBase
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            var seen = new Dictionary<string, string>();
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset == null || string.IsNullOrEmpty(asset.Id)) continue;
                if (seen.TryGetValue(asset.Id, out var existing))
                    errors.Add($"{typeof(T).Name} Id '{asset.Id}' duplicated: {path} vs {existing}");
                else seen[asset.Id] = path;
            }
        }

        static void ValidateBuilding(BuildingDef b, string path, List<string> errors)
        {
            if (string.IsNullOrEmpty(b.Id)) errors.Add($"BuildingDef {path} has empty Id");
            if (string.IsNullOrEmpty(b.DisplayNameKey)) errors.Add($"BuildingDef {b.Id} has empty DisplayNameKey ({path})");
            if (b.Category == "generator" && b.OutputResource == null)
                errors.Add($"BuildingDef {b.Id} (generator) has null OutputResource");
            if (b.Category == "processor")
            {
                if (b.InputResource == null) errors.Add($"BuildingDef {b.Id} (processor) has null InputResource");
                if (b.OutputResource == null) errors.Add($"BuildingDef {b.Id} (processor) has null OutputResource");
            }
            if (b.BaseCycleSeconds < 0f) errors.Add($"BuildingDef {b.Id} has negative BaseCycleSeconds");
            if (b.CostGrowth < 1f) errors.Add($"BuildingDef {b.Id} CostGrowth {b.CostGrowth} < 1 (would shrink cost)");
            if (b.MaxLevel < 1) errors.Add($"BuildingDef {b.Id} MaxLevel < 1");
        }

        static void ValidateResource(ResourceDef r, string path, List<string> errors)
        {
            if (string.IsNullOrEmpty(r.Id)) errors.Add($"ResourceDef {path} has empty Id");
            if (r.BaseSellPrice <= 0) errors.Add($"ResourceDef {r.Id} has non-positive BaseSellPrice");
            if (r.Stage < 0 || r.Stage > 2) errors.Add($"ResourceDef {r.Id} Stage out of [0,2]");
        }

        static void ValidateNpc(NPCDef n, string path, List<string> errors)
        {
            if (string.IsNullOrEmpty(n.Id)) errors.Add($"NPCDef {path} has empty Id");
            if (n.BaseCapacity <= 0) errors.Add($"NPCDef {n.Id} BaseCapacity <= 0");
            if (n.BaseSpeed <= 0f) errors.Add($"NPCDef {n.Id} BaseSpeed <= 0");
            if (n.BaseEfficiency <= 0f) errors.Add($"NPCDef {n.Id} BaseEfficiency <= 0");
            if (n.HireCostGold < 0) errors.Add($"NPCDef {n.Id} negative HireCostGold");
        }

        static void ValidateIap(IapProductDef p, string path, List<string> errors)
        {
            if (string.IsNullOrEmpty(p.Id)) errors.Add($"IapProductDef {path} has empty Id");
            if (string.IsNullOrEmpty(p.Sku)) errors.Add($"IapProductDef {p.Id} has empty Sku");
            if (p.FallbackPriceUsd <= 0f) errors.Add($"IapProductDef {p.Id} non-positive FallbackPriceUsd");
        }

        static void ValidateTutorialStep(TutorialStep s, string path, List<string> errors)
        {
            if (string.IsNullOrEmpty(s.Id)) errors.Add($"TutorialStep {path} has empty Id");
            if (string.IsNullOrEmpty(s.MessageKey)) errors.Add($"TutorialStep {s.Id} has empty MessageKey");
            if (s.Advance == TutorialAdvance.BuildingClicked && string.IsNullOrEmpty(s.ExpectedBuildingId))
                Debug.LogWarning($"[AssetValidator] TutorialStep {s.Id} BuildingClicked w/ no ExpectedBuildingId — matches any building");
        }
    }
}

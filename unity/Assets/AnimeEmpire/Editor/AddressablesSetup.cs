using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace AnimeEmpire.Editor
{
    public static class AddressablesSetup
    {
        public const string BootstrapPrefabPath = "Assets/AnimeEmpire/Resources/Bootstrap.prefab";
        public const string BootstrapAddress = "Bootstrap";
        public const string BootstrapGroupName = "Bootstrap";

        [MenuItem("Tools/Anime Empire/Addressables → Setup Bootstrap group")]
        public static void SetupBootstrapGroup()
        {
            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            if (settings == null)
            {
                Debug.LogError("[AddressablesSetup] Could not create AddressableAssetSettings — verify com.unity.addressables installed.");
                return;
            }

            var group = settings.FindGroup(BootstrapGroupName);
            if (group == null)
            {
                var template = settings.DefaultGroup;
                group = settings.CreateGroup(BootstrapGroupName,
                    setAsDefaultGroup: false,
                    readOnly: false,
                    postEvent: true,
                    schemasToCopy: template != null ? template.Schemas : null);
            }

            var guid = AssetDatabase.AssetPathToGUID(BootstrapPrefabPath);
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogWarning($"[AddressablesSetup] Bootstrap prefab not found at {BootstrapPrefabPath} — run Build Phase 1 Content first.");
                return;
            }
            var entry = settings.CreateOrMoveEntry(guid, group);
            entry.address = BootstrapAddress;
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            Debug.Log($"[AddressablesSetup] Bootstrap.prefab → Addressable group '{BootstrapGroupName}', address '{BootstrapAddress}'.");
        }
    }
}

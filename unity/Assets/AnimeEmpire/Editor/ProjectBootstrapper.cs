using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace AnimeEmpire.Editor
{
    [InitializeOnLoad]
    public static class ProjectBootstrapper
    {
        const string AppliedKey = "AnimeEmpire.ProjectBootstrapper.Applied.v1";

        static ProjectBootstrapper()
        {
            EditorApplication.delayCall += Apply;
        }

        static void Apply()
        {
            if (SessionState.GetBool(AppliedKey, false)) return;
            SessionState.SetBool(AppliedKey, true);

            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.companyName = "Anime Empire";
            PlayerSettings.productName = "Anime Empire";
            PlayerSettings.bundleVersion = "0.0.1";

            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.useAnimatedAutorotation = false;

            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, "com.animeempire.app");
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.animeempire.app");

            EditorSettings.serializationMode = SerializationMode.ForceText;
            EditorSettings.assetPipelineMode = AssetPipelineMode.Version2;

            Debug.Log("[AnimeEmpire] Project bootstrap settings applied. Run Tools → Anime Empire → Build Phase 1 Content to author SOs/scenes/prefabs.");
        }
    }
}

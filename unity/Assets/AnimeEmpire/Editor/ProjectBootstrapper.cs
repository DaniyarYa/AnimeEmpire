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

            // IL2CPP + ARM64 for Android (drop ARMv7); IL2CPP for iOS.
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.Android.forceInternetPermission = true;

            PlayerSettings.SetScriptingBackend(NamedBuildTarget.iOS, ScriptingImplementation.IL2CPP);
            PlayerSettings.iOS.targetOSVersionString = "13.0";

            EditorSettings.serializationMode = SerializationMode.ForceText;

            Debug.Log("[AnimeEmpire] Project bootstrap settings applied. Run Tools → Anime Empire → Build Phase 1 Content to author SOs/scenes/prefabs.");
        }
    }
}

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace AnimeEmpire.Editor
{
    public static class BuildScript
    {
        public static void BuildAndroid()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            PlayerSettings.SetScriptingBackend(NamedBuildTargetFor(BuildTarget.Android), UnityEditor.ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            EditorUserBuildSettings.buildAppBundle = true;
            Run(BuildTarget.Android, ResolveAndroidOutput());
        }

        public static void BuildIOS()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            PlayerSettings.SetScriptingBackend(NamedBuildTargetFor(BuildTarget.iOS), UnityEditor.ScriptingImplementation.IL2CPP);
            PlayerSettings.iOS.targetOSVersionString = "13.0";
            Run(BuildTarget.iOS, ResolveIOSOutput());
        }

        static void Run(BuildTarget target, string output)
        {
            var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
            if (scenes.Length == 0)
                throw new Exception("[BuildScript] no scenes in build settings — run Tools → Anime Empire → Build Phase 1 Content first");
            var dir = Path.GetDirectoryName(output);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            var opts = new BuildPlayerOptions
            {
                scenes = scenes,
                target = target,
                locationPathName = output,
                options = BuildOptions.None,
            };
            var report = BuildPipeline.BuildPlayer(opts);
            var summary = report.summary;
            Debug.Log($"[BuildScript] {target}: {summary.result} ({summary.totalSize} bytes) → {output}");
            if (summary.result != BuildResult.Succeeded)
                throw new Exception($"[BuildScript] {target} build {summary.result}");
        }

        static UnityEditor.Build.NamedBuildTarget NamedBuildTargetFor(BuildTarget t) => t switch
        {
            BuildTarget.Android => UnityEditor.Build.NamedBuildTarget.Android,
            BuildTarget.iOS => UnityEditor.Build.NamedBuildTarget.iOS,
            _ => UnityEditor.Build.NamedBuildTarget.Standalone,
        };

        static string ResolveAndroidOutput()
        {
            var env = Environment.GetEnvironmentVariable("BUILD_OUTPUT");
            if (!string.IsNullOrEmpty(env)) return env;
            return EditorUserBuildSettings.buildAppBundle
                ? "build/Android/AnimeEmpire.aab"
                : "build/Android/AnimeEmpire.apk";
        }

        static string ResolveIOSOutput()
        {
            var env = Environment.GetEnvironmentVariable("BUILD_OUTPUT");
            return string.IsNullOrEmpty(env) ? "build/iOS" : env;
        }
    }
}

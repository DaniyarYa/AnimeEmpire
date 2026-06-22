using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;

namespace AnimeEmpire.Editor
{
    public static class BuildScript
    {
        public static void BuildAndroid()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            PlayerSettings.SetScriptingBackend(NamedBuildTargetFor(BuildTarget.Android), UnityEditor.ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = UnityEditor.AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = UnityEditor.AndroidSdkVersions.AndroidApiLevel25;
            PlayerSettings.Android.targetSdkVersion = UnityEditor.AndroidSdkVersions.AndroidApiLevelAuto;
            EditorUserBuildSettings.buildAppBundle = true;
            ApplyVersionSuffix();
            Run(BuildTarget.Android, ResolveAndroidOutput());
        }

        public static void BuildIOS()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            PlayerSettings.SetScriptingBackend(NamedBuildTargetFor(BuildTarget.iOS), UnityEditor.ScriptingImplementation.IL2CPP);
            PlayerSettings.iOS.targetOSVersionString = "13.0";
            ApplyVersionSuffix();
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
            WriteBuildInfo(target, output, summary);
            if (summary.result != BuildResult.Succeeded)
                throw new Exception($"[BuildScript] {target} build {summary.result}");
        }

        static void WriteBuildInfo(BuildTarget target, string output, BuildSummary summary)
        {
            var dir = Path.GetDirectoryName(output);
            if (string.IsNullOrEmpty(dir)) dir = "build";
            Directory.CreateDirectory(dir);
            var infoPath = Path.Combine(dir, "build_info.json");
            var info = new
            {
                target = target.ToString(),
                result = summary.result.ToString(),
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                commit_hash = GitRev("rev-parse --short HEAD") ?? Environment.GetEnvironmentVariable("GITHUB_SHA")?.Substring(0, Math.Min(7, Environment.GetEnvironmentVariable("GITHUB_SHA")?.Length ?? 0)),
                branch = GitRev("rev-parse --abbrev-ref HEAD") ?? Environment.GetEnvironmentVariable("GITHUB_REF_NAME"),
                output_path = output,
                bundle_version = PlayerSettings.bundleVersion,
                bundle_identifier = PlayerSettings.applicationIdentifier,
                scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
                size_bytes = (long)summary.totalSize,
                unity_version = UnityEngine.Application.unityVersion,
            };
            File.WriteAllText(infoPath, JsonConvert.SerializeObject(info, Formatting.Indented));
            Debug.Log($"[BuildScript] wrote {infoPath}");
        }

        static void ApplyVersionSuffix()
        {
            if (Environment.GetEnvironmentVariable("BUILD_VERSION_SUFFIX") != "1") return;
            var hash = GitRev("rev-parse --short HEAD");
            if (string.IsNullOrEmpty(hash)) return;
            var baseVersion = PlayerSettings.bundleVersion;
            if (baseVersion.Contains("+")) baseVersion = baseVersion.Substring(0, baseVersion.IndexOf('+'));
            PlayerSettings.bundleVersion = $"{baseVersion}+{hash}";
            Debug.Log($"[BuildScript] bundle version → {PlayerSettings.bundleVersion}");
        }

        static string GitRev(string args)
        {
            try
            {
                var psi = new ProcessStartInfo("git", args)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = UnityEngine.Application.dataPath + "/..",
                };
                using var p = Process.Start(psi);
                if (p == null) return null;
                var stdout = p.StandardOutput.ReadToEnd().Trim();
                p.WaitForExit(2000);
                return p.ExitCode == 0 && !string.IsNullOrEmpty(stdout) ? stdout : null;
            }
            catch
            {
                return null;
            }
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

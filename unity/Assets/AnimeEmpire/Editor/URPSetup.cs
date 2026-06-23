using System.IO;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AnimeEmpire.Editor
{
    public static class URPSetup
    {
        public const string SettingsFolder = "Assets/AnimeEmpire/Settings/URP";
        public const string RendererPath = SettingsFolder + "/URP-Mobile-Renderer.asset";
        public const string PipelineAssetPath = SettingsFolder + "/URP-Mobile.asset";

        [MenuItem("Tools/Anime Empire/Setup URP Pipeline")]
        public static void Setup()
        {
            EnsureFolder(SettingsFolder);

            // Renderer asset (Universal Renderer).
            var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(RendererPath);
            if (renderer == null)
            {
                renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(renderer, RendererPath);
            }

            // Pipeline asset.
            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelineAssetPath);
            if (pipeline == null)
            {
                pipeline = UniversalRenderPipelineAsset.Create(renderer);
                AssetDatabase.CreateAsset(pipeline, PipelineAssetPath);
            }
            EditorUtility.SetDirty(pipeline);
            EditorUtility.SetDirty(renderer);

            // Assign to Graphics settings (active render pipeline).
            GraphicsSettings.defaultRenderPipeline = pipeline;

            // Assign to all Quality levels.
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                QualitySettings.SetQualityLevel(i, false);
                QualitySettings.renderPipeline = pipeline;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[URPSetup] URP pipeline authored at {PipelineAssetPath} + assigned to Graphics + Quality levels.");
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

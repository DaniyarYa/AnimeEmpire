using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AnimeEmpire.Editor
{
    public static class AnimatorControllerBuilder
    {
        const string PlayerAvatarFolder = "Assets/AnimeEmpire/Art/Characters/PlayerAvatar/v0";
        const string ControllersFolder = "Assets/AnimeEmpire/ScriptableObjects/AnimatorControllers";
        const string PlayerControllerPath = ControllersFolder + "/PlayerController.controller";
        const string NpcControllerPath = ControllersFolder + "/NpcController.controller";

        static readonly string[] CanonicalStates =
        {
            PlayerAnimationStateNames.Idle,
            PlayerAnimationStateNames.Walk,
            PlayerAnimationStateNames.Run,
            PlayerAnimationStateNames.WorkSit,
            PlayerAnimationStateNames.WorkGather,
            PlayerAnimationStateNames.WorkStand,
            PlayerAnimationStateNames.CarryWalk,
            PlayerAnimationStateNames.Celebrate,
        };

        [MenuItem("Tools/Anime Empire/Rebuild Animator Controllers")]
        public static void RebuildBoth()
        {
            EnsureFolder(ControllersFolder);
            var clips = LoadCanonicalClips();
            BuildController(PlayerControllerPath, clips);
            BuildController(NpcControllerPath, clips);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[AnimatorControllerBuilder] built {PlayerControllerPath} + {NpcControllerPath} with {clips.Count} clips.");
        }

        public static AnimatorController BuildController(string path, Dictionary<string, AnimationClip> clips)
        {
            var existing = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (existing != null) AssetDatabase.DeleteAsset(path);
            var controller = AnimatorController.CreateAnimatorControllerAtPath(path);
            var sm = controller.layers[0].stateMachine;

            // Wipe default states the API may have created.
            foreach (var s in new List<ChildAnimatorState>(sm.states)) sm.RemoveState(s.state);

            // Add states in canonical order. First state becomes default.
            foreach (var name in CanonicalStates)
            {
                var state = sm.AddState(name);
                if (clips.TryGetValue(name, out var clip)) state.motion = clip;
            }
            // Default state = idle if present.
            foreach (var st in sm.states)
            {
                if (st.state.name == PlayerAnimationStateNames.Idle)
                {
                    sm.defaultState = st.state;
                    break;
                }
            }
            EditorUtility.SetDirty(controller);
            return controller;
        }

        public static Dictionary<string, AnimationClip> LoadCanonicalClips()
        {
            var map = new Dictionary<string, AnimationClip>();
            foreach (var (fileName, stateName) in new (string, string)[]
            {
                ("idle",         PlayerAnimationStateNames.Idle),
                ("walk",         PlayerAnimationStateNames.Walk),
                ("run",          PlayerAnimationStateNames.Run),
                ("carry_walk",   PlayerAnimationStateNames.CarryWalk),
                ("work_harvest", PlayerAnimationStateNames.WorkGather),
                ("celebrate",    PlayerAnimationStateNames.Celebrate),
            })
            {
                var path = $"{PlayerAvatarFolder}/Animations/{fileName}.fbx";
                var clip = FindClipInFbx(path, stateName);
                if (clip != null) map[stateName] = clip;
            }

            // work_sit / work_stand currently share work_gather visual until dedicated FBX clips exist.
            if (map.TryGetValue(PlayerAnimationStateNames.WorkGather, out var gather))
            {
                if (!map.ContainsKey(PlayerAnimationStateNames.WorkSit)) map[PlayerAnimationStateNames.WorkSit] = gather;
                if (!map.ContainsKey(PlayerAnimationStateNames.WorkStand)) map[PlayerAnimationStateNames.WorkStand] = gather;
            }
            return map;
        }

        static AnimationClip FindClipInFbx(string fbxPath, string preferredName)
        {
            if (!File.Exists(fbxPath)) return null;
            AnimationClip fallback = null;
            foreach (var obj in AssetDatabase.LoadAllAssetsAtPath(fbxPath))
            {
                if (obj is AnimationClip clip && !clip.name.StartsWith("__preview__"))
                {
                    if (clip.name == preferredName) return clip;
                    fallback ??= clip;
                }
            }
            return fallback;
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

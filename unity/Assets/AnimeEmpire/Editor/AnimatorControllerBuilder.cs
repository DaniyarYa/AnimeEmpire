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

        // Float parameter — Player movement speed normalized [0..1].
        public const string ParamSpeed = "Speed";
        // Bool parameter — NPC carrying load.
        public const string ParamCarrying = "Carrying";
        // Trigger parameters — discrete work cycle steps.
        public const string TriggerSit = "Sit";
        public const string TriggerGather = "Gather";
        public const string TriggerStand = "Stand";
        public const string TriggerCelebrate = "Celebrate";
        public const string TriggerIdle = "ToIdle";

        // Speed thresholds for locomotion auto-transitions.
        const float WalkThreshold = 0.05f;
        const float RunThreshold = 0.85f;

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
            Debug.Log($"[AnimatorControllerBuilder] built {PlayerControllerPath} + {NpcControllerPath} with {clips.Count} clips + parameters + transitions.");
        }

        public static AnimatorController BuildController(string path, Dictionary<string, AnimationClip> clips)
        {
            var existing = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (existing != null) AssetDatabase.DeleteAsset(path);
            var controller = AnimatorController.CreateAnimatorControllerAtPath(path);

            // Parameters.
            controller.AddParameter(ParamSpeed, AnimatorControllerParameterType.Float);
            controller.AddParameter(ParamCarrying, AnimatorControllerParameterType.Bool);
            controller.AddParameter(TriggerSit, AnimatorControllerParameterType.Trigger);
            controller.AddParameter(TriggerGather, AnimatorControllerParameterType.Trigger);
            controller.AddParameter(TriggerStand, AnimatorControllerParameterType.Trigger);
            controller.AddParameter(TriggerCelebrate, AnimatorControllerParameterType.Trigger);
            controller.AddParameter(TriggerIdle, AnimatorControllerParameterType.Trigger);

            var sm = controller.layers[0].stateMachine;
            foreach (var s in new List<ChildAnimatorState>(sm.states)) sm.RemoveState(s.state);

            var stateLookup = new Dictionary<string, AnimatorState>();
            foreach (var name in CanonicalStates)
            {
                var state = sm.AddState(name);
                if (clips.TryGetValue(name, out var clip)) state.motion = clip;
                stateLookup[name] = state;
            }
            sm.defaultState = stateLookup[PlayerAnimationStateNames.Idle];

            BuildLocomotionTransitions(stateLookup);
            BuildWorkCycleTransitions(stateLookup);
            BuildCarryTransitions(stateLookup);
            BuildCelebrateTransitions(stateLookup);
            BuildAnyStateTriggers(sm, stateLookup);

            EditorUtility.SetDirty(controller);
            return controller;
        }

        // idle ↔ walk ↔ run by Speed float w/ hysteresis-ish thresholds.
        static void BuildLocomotionTransitions(Dictionary<string, AnimatorState> s)
        {
            var idle = s[PlayerAnimationStateNames.Idle];
            var walk = s[PlayerAnimationStateNames.Walk];
            var run = s[PlayerAnimationStateNames.Run];

            // idle → walk
            var t = idle.AddTransition(walk);
            ConfigureSpeedTransition(t, AnimatorConditionMode.Greater, WalkThreshold);

            // walk → idle
            t = walk.AddTransition(idle);
            ConfigureSpeedTransition(t, AnimatorConditionMode.Less, WalkThreshold);

            // walk → run
            t = walk.AddTransition(run);
            ConfigureSpeedTransition(t, AnimatorConditionMode.Greater, RunThreshold);

            // run → walk
            t = run.AddTransition(walk);
            ConfigureSpeedTransition(t, AnimatorConditionMode.Less, RunThreshold);
        }

        // work_sit auto-progresses to work_gather after one loop (exit time).
        // work_gather loops by default.
        // work_stand → either carry_walk (if Carrying) or idle.
        static void BuildWorkCycleTransitions(Dictionary<string, AnimatorState> s)
        {
            var sit = s[PlayerAnimationStateNames.WorkSit];
            var gather = s[PlayerAnimationStateNames.WorkGather];
            var stand = s[PlayerAnimationStateNames.WorkStand];
            var idle = s[PlayerAnimationStateNames.Idle];
            var carry = s[PlayerAnimationStateNames.CarryWalk];

            // sit → gather automatically after clip finishes.
            var t = sit.AddTransition(gather);
            t.hasExitTime = true;
            t.exitTime = 0.95f;
            t.duration = 0.15f;
            t.hasFixedDuration = true;

            // gather → stand on Stand trigger.
            t = gather.AddTransition(stand);
            t.AddCondition(AnimatorConditionMode.If, 0, TriggerStand);
            t.hasExitTime = false;
            t.duration = 0.1f;
            t.hasFixedDuration = true;

            // stand → carry if Carrying flag set, else → idle. Both fire on exit time.
            t = stand.AddTransition(carry);
            t.hasExitTime = true;
            t.exitTime = 0.9f;
            t.duration = 0.15f;
            t.hasFixedDuration = true;
            t.AddCondition(AnimatorConditionMode.If, 0, ParamCarrying);

            t = stand.AddTransition(idle);
            t.hasExitTime = true;
            t.exitTime = 0.9f;
            t.duration = 0.15f;
            t.hasFixedDuration = true;
            t.AddCondition(AnimatorConditionMode.IfNot, 0, ParamCarrying);
        }

        // carry_walk → idle when not carrying anymore (Carrying = false).
        static void BuildCarryTransitions(Dictionary<string, AnimatorState> s)
        {
            var carry = s[PlayerAnimationStateNames.CarryWalk];
            var idle = s[PlayerAnimationStateNames.Idle];

            var t = carry.AddTransition(idle);
            t.AddCondition(AnimatorConditionMode.IfNot, 0, ParamCarrying);
            t.hasExitTime = false;
            t.duration = 0.2f;
            t.hasFixedDuration = true;
        }

        // celebrate one-shot → idle on clip finish.
        static void BuildCelebrateTransitions(Dictionary<string, AnimatorState> s)
        {
            var celebrate = s[PlayerAnimationStateNames.Celebrate];
            var idle = s[PlayerAnimationStateNames.Idle];
            var t = celebrate.AddTransition(idle);
            t.hasExitTime = true;
            t.exitTime = 0.95f;
            t.duration = 0.2f;
            t.hasFixedDuration = true;
        }

        // Any State triggers — let code fire Sit/Gather/Celebrate/ToIdle from anywhere.
        static void BuildAnyStateTriggers(AnimatorStateMachine sm, Dictionary<string, AnimatorState> s)
        {
            void Add(string trigger, AnimatorState target)
            {
                var t = sm.AddAnyStateTransition(target);
                t.AddCondition(AnimatorConditionMode.If, 0, trigger);
                t.duration = 0.15f;
                t.hasFixedDuration = true;
                t.canTransitionToSelf = false;
            }

            Add(TriggerSit, s[PlayerAnimationStateNames.WorkSit]);
            Add(TriggerGather, s[PlayerAnimationStateNames.WorkGather]);
            Add(TriggerCelebrate, s[PlayerAnimationStateNames.Celebrate]);
            Add(TriggerIdle, s[PlayerAnimationStateNames.Idle]);
        }

        static void ConfigureSpeedTransition(AnimatorStateTransition t, AnimatorConditionMode mode, float threshold)
        {
            t.AddCondition(mode, threshold, ParamSpeed);
            t.hasExitTime = false;
            t.duration = 0.15f;
            t.hasFixedDuration = true;
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

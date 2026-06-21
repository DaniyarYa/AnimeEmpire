using UnityEditor;
using UnityEngine;

namespace AnimeEmpire.Editor
{
    public class PlayerAssetPostprocessor : AssetPostprocessor
    {
        const string PlayerAvatarFolder = "Assets/AnimeEmpire/Art/Characters/PlayerAvatar";
        const string PlayerAvatarRig = PlayerAvatarFolder + "/v0/player_avatar.fbx";

        bool InPlayerFolder => assetPath != null && assetPath.StartsWith(PlayerAvatarFolder);

        void OnPreprocessModel()
        {
            if (!InPlayerFolder) return;
            var importer = (ModelImporter)assetImporter;

            importer.globalScale = 1f;
            importer.useFileScale = true;
            importer.meshCompression = ModelImporterMeshCompression.Off;
            importer.isReadable = false;
            importer.optimizeMeshPolygons = true;
            importer.optimizeMeshVertices = true;
            importer.importBlendShapes = false;
            importer.addCollider = false;
            importer.materialImportMode = ModelImporterMaterialImportMode.None;
            importer.animationType = ModelImporterAnimationType.Generic;
            importer.optimizeGameObjects = true;

            bool isRig = assetPath == PlayerAvatarRig;
            if (isRig)
            {
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                importer.importAnimation = false;
            }
            else
            {
                importer.importAnimation = true;
                var rigAvatar = AssetDatabase.LoadAssetAtPath<Avatar>(PlayerAvatarRig);
                if (rigAvatar != null)
                {
                    importer.avatarSetup = ModelImporterAvatarSetup.CopyFromOther;
                    importer.sourceAvatar = rigAvatar;
                }
            }
        }

        void OnPostprocessAnimation(GameObject root, AnimationClip clip)
        {
            if (!InPlayerFolder) return;
            string fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            // Normalize clip name to short canonical state name matching Animator state hashes.
            // Original FBX clip names look like "Armature|Armature|<Name>|baselayer" — strip and map.
            clip.name = MapToCanonicalState(fileName, clip.name);
            // Loop flags by canonical name.
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = IsLooping(clip.name);
            AnimationUtility.SetAnimationClipSettings(clip, settings);
        }

        static string MapToCanonicalState(string fileName, string clipName)
        {
            // FileName is authoritative (one clip per FBX in our pipeline).
            return fileName switch
            {
                "idle" => PlayerAnimationStateNames.Idle,
                "walk" => PlayerAnimationStateNames.Walk,
                "walk_inplace" => PlayerAnimationStateNames.WalkInplace,
                "run" => PlayerAnimationStateNames.Run,
                "carry_walk" => PlayerAnimationStateNames.CarryWalk,
                "work_harvest" => PlayerAnimationStateNames.WorkGather,
                "celebrate" => PlayerAnimationStateNames.Celebrate,
                _ => clipName,
            };
        }

        static bool IsLooping(string canonicalName) => canonicalName switch
        {
            PlayerAnimationStateNames.Idle => true,
            PlayerAnimationStateNames.Walk => true,
            PlayerAnimationStateNames.WalkInplace => true,
            PlayerAnimationStateNames.Run => true,
            PlayerAnimationStateNames.CarryWalk => true,
            PlayerAnimationStateNames.WorkGather => true,
            _ => false,
        };
    }

    public static class PlayerAnimationStateNames
    {
        public const string Idle = "idle";
        public const string Walk = "walk";
        public const string WalkInplace = "walk_inplace";
        public const string Run = "run";
        public const string CarryWalk = "carry_walk";
        public const string WorkSit = "work_sit";
        public const string WorkGather = "work_gather";
        public const string WorkStand = "work_stand";
        public const string Celebrate = "celebrate";
    }
}

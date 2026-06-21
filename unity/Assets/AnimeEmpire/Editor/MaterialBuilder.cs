using UnityEditor;
using UnityEngine;

namespace AnimeEmpire.Editor
{
    public static class MaterialBuilder
    {
        public const string PlayerMaterialPath = "Assets/AnimeEmpire/Art/Characters/PlayerAvatar/v0/Materials/PlayerAvatarMat.mat";
        public const string DiffuseTexturePath = "Assets/AnimeEmpire/Art/Characters/PlayerAvatar/v0/Textures/diffuse_2048.png";

        public static Material EnsurePlayerMaterial()
        {
            EnsureFolder("Assets/AnimeEmpire/Art/Characters/PlayerAvatar/v0/Materials");
            var existing = AssetDatabase.LoadAssetAtPath<Material>(PlayerMaterialPath);
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                Debug.LogWarning("[MaterialBuilder] URP/Lit shader missing — material left unset. Ensure URP package imported.");
                return existing;
            }
            var mat = existing != null ? existing : new Material(shader) { name = "PlayerAvatarMat" };
            if (existing == null) AssetDatabase.CreateAsset(mat, PlayerMaterialPath);
            mat.shader = shader;
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(DiffuseTexturePath);
            if (tex != null)
            {
                if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", tex);
                if (mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", tex);
            }
            EditorUtility.SetDirty(mat);
            return mat;
        }

        public static void AssignToRenderers(GameObject root, Material mat)
        {
            if (root == null || mat == null) return;
            foreach (var r in root.GetComponentsInChildren<Renderer>(true))
            {
                var arr = new Material[r.sharedMaterials.Length];
                for (int i = 0; i < arr.Length; i++) arr[i] = mat;
                r.sharedMaterials = arr;
            }
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

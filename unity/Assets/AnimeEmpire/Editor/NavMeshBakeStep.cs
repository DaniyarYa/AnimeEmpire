using UnityEditor;
using UnityEditor.AI;
using UnityEngine;

namespace AnimeEmpire.Editor
{
    public static class NavMeshBakeStep
    {
        public static void MarkNavStatic(GameObject go)
        {
            if (go == null) return;
            GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.NavigationStatic | GameObjectUtility.GetStaticEditorFlags(go));
        }

        public static void Bake()
        {
            NavMeshBuilder.ClearAllNavMeshes();
            NavMeshBuilder.BuildNavMesh();
            Debug.Log("[NavMeshBakeStep] NavMesh baked for active scene.");
        }
    }
}

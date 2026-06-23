using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace AnimeEmpire.Editor
{
    /// Unity 6 AI Navigation package — bake via NavMeshSurface component
    /// instead of deprecated Static-flag + NavMeshBuilder.BuildNavMesh().
    public static class NavMeshBakeStep
    {
        public static NavMeshSurface AddSurfaceAndBake(GameObject host)
        {
            if (host == null) return null;
            var surface = host.GetComponent<NavMeshSurface>();
            if (surface == null) surface = host.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.All;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.layerMask = ~0;
            surface.BuildNavMesh();
            Debug.Log($"[NavMeshBakeStep] surface baked on {host.name}");
            return surface;
        }
    }
}

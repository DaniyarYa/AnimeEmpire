using UnityEngine;

namespace AnimeEmpire.Core
{
    public static class AppBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Spawn()
        {
            if (Object.FindFirstObjectByType<GameState>() != null) return;

            var prefab = Resources.Load<GameObject>("Bootstrap");
            if (prefab == null)
            {
                Debug.LogWarning("[AppBootstrap] Resources/Bootstrap.prefab not found — services will not be available. Run Tools → Anime Empire → Build Phase 1 Content.");
                return;
            }
            var go = Object.Instantiate(prefab);
            go.name = "Bootstrap";
            Object.DontDestroyOnLoad(go);
        }
    }
}

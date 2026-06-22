using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AnimeEmpire.Core
{
    public static class AppBootstrap
    {
        public const string BootstrapAddress = "Bootstrap";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Spawn()
        {
            if (Object.FindAnyObjectByType<GameState>() != null) return;

            GameObject prefab = LoadFromAddressables() ?? Resources.Load<GameObject>(BootstrapAddress);
            if (prefab == null)
            {
                Debug.LogWarning("[AppBootstrap] Bootstrap prefab missing from Addressables AND Resources/. Run Tools → Anime Empire → Build Phase 1 Content.");
                return;
            }
            var go = Object.Instantiate(prefab);
            go.name = "Bootstrap";
            Object.DontDestroyOnLoad(go);
        }

        static GameObject LoadFromAddressables()
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(BootstrapAddress);
                var result = handle.WaitForCompletion();
                return result;
            }
            catch
            {
                // Addressables not initialized OR address not authored — fall back to Resources.
                return null;
            }
        }
    }
}

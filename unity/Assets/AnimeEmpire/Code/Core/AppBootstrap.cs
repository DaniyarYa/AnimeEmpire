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

            // Resources first (default, no runtime data dependency). Addressables
            // only consulted when Resources misses — Phase 2 DLC bundles plug in here.
            GameObject prefab = Resources.Load<GameObject>(BootstrapAddress) ?? LoadFromAddressables();
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
            // Skip Addressables if no settings authored — avoids noisy
            // "RuntimeData is null" error cascade.
            if (Addressables.ResourceLocators == null) return null;
            try
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(BootstrapAddress);
                var result = handle.WaitForCompletion();
                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}

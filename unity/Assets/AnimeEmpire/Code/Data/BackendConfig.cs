using UnityEngine;

namespace AnimeEmpire.Data
{
    [CreateAssetMenu(fileName = "BackendConfig", menuName = "Anime Empire/Backend Config")]
    public class BackendConfig : ScriptableObject
    {
        [Tooltip("HTTP(S) URL returning RemoteConfig JSON. Empty = use built-in defaults only.")]
        public string ConfigUrl = "https://animeempire-a8eee.web.app/config.json";

        [Tooltip("HTTP request timeout (seconds).")]
        public int RequestTimeoutSeconds = 5;
    }
}

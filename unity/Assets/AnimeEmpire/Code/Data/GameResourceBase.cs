using UnityEngine;

namespace AnimeEmpire.Data
{
    public abstract class GameResourceBase : ScriptableObject
    {
        [Tooltip("Unique snake_case identifier. Used as dictionary key, save key, RemoteConfig override path.")]
        public string Id = "";

        [Tooltip("i18n key for display name (e.g. building.wheat_farm.name).")]
        public string DisplayNameKey = "";

        void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(Id))
                Id = name.ToLowerInvariant();
        }
    }
}

using UnityEngine;

namespace AnimeEmpire.Data
{
    [CreateAssetMenu(fileName = "ResourceDef", menuName = "Anime Empire/Resource Def")]
    public class ResourceDef : GameResourceBase
    {
        [Tooltip("Base sell price (gold per unit).")]
        public int BaseSellPrice = 1;

        [Tooltip("Icon for UI (inventory, shop, achievements).")]
        public Sprite Icon;

        [Tooltip("Processing stage: 0=raw, 1=intermediate, 2=final.")]
        [Range(0, 2)] public int Stage = 0;

        [Tooltip("Rarity tier: common | rare | epic | legendary.")]
        public string Tier = "common";
    }
}

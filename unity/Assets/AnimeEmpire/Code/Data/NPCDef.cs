using UnityEngine;

namespace AnimeEmpire.Data
{
    [CreateAssetMenu(fileName = "NPCDef", menuName = "Anime Empire/NPC Def")]
    public class NPCDef : GameResourceBase
    {
        [Tooltip("Category: gatherer | carrier | operator | specialist | manager.")]
        public string Category = "gatherer";

        [Tooltip("Rarity: common | rare | epic | legendary.")]
        public string Rarity = "common";

        [Tooltip("Movement speed (units/sec).")]
        public float BaseSpeed = 2.0f;

        [Tooltip("Carry capacity (max units before delivering).")]
        public int BaseCapacity = 5;

        [Tooltip("Work efficiency factor (0..1+) — divides cycle time.")]
        public float BaseEfficiency = 0.75f;

        public int HireCostGold = 1000;

        [Tooltip("Empty = universal; otherwise category of building NPC is attached to.")]
        public string AttachedBuildingCategory = "";

        public GameObject ModelPrefab;
        public Sprite Portrait;
    }
}

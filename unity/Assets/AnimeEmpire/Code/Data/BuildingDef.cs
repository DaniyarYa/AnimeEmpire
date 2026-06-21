using UnityEngine;

namespace AnimeEmpire.Data
{
    [CreateAssetMenu(fileName = "BuildingDef", menuName = "Anime Empire/Building Def")]
    public class BuildingDef : GameResourceBase
    {
        [Tooltip("Category: generator | processor | service.")]
        public string Category = "generator";

        public ResourceDef InputResource;
        public int InputAmount = 0;

        public ResourceDef OutputResource;
        public int OutputAmount = 1;

        [Tooltip("Cycle duration at level 1 (seconds).")]
        public float BaseCycleSeconds = 1f;

        [Tooltip("Purchase cost at level 1 (gold).")]
        public int BaseCostGold = 100;

        [Tooltip("Upgrade cost growth: cost(N) = base * growth^(N-1). 1.12 early / 1.15 mid / 1.18 late.")]
        public float CostGrowth = 1.15f;

        public int MaxLevel = 25;
        public int UnlockLevel = 1;
        public int NpcSlots = 1;
    }
}

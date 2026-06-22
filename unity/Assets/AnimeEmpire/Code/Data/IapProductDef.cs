using UnityEngine;

namespace AnimeEmpire.Data
{
    [CreateAssetMenu(fileName = "IapProductDef", menuName = "Anime Empire/IAP Product Def")]
    public class IapProductDef : GameResourceBase
    {
        [Tooltip("Store SKU (matches App Store Connect / Play Console).")]
        public string Sku = "";

        [Tooltip("Product kind: consumable | non_consumable | subscription.")]
        public string Kind = "consumable";

        [Tooltip("Display price USD, fallback when store unavailable.")]
        public float FallbackPriceUsd = 0.99f;

        public string DescriptionKey = "";

        [Header("Grants")]
        public int GemGrant;
        public int GoldGrant;
        public bool NoAdsUnlock;
        public bool BattlePassUnlock;
    }
}

using System;
using System.Collections.Generic;
using AnimeEmpire.Core;
using AnimeEmpire.Data;
using AnimeEmpire.Economy;
using UnityEngine;

namespace AnimeEmpire.Backend
{
    public class MonetizationService : MonoBehaviour
    {
        public static MonetizationService Instance { get; private set; }

        [SerializeField] IapCatalog _catalog;

        public IapCatalog Catalog => _catalog;

        public event Action<string, Dictionary<string, object>> PurchaseCompleted;
        public event Action<string, string> PurchaseFailed;
        public event Action<string, Dictionary<string, object>> AdCompleted;
        public event Action<bool> SubscriptionChanged;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (_catalog == null) _catalog = Resources.Load<IapCatalog>("IapCatalog");
            Debug.Log($"[MonetizationService] ready (stub, {(_catalog != null ? _catalog.Products.Count : 0)} products)");
        }

        void OnDestroy() { if (Instance == this) Instance = null; }

        public void Purchase(string sku)
        {
            Debug.LogWarning($"[MonetizationService] purchase stub for sku={sku}");
            EventBus.RaiseIapCompleted(sku, false);
            PurchaseFailed?.Invoke(sku, "not_implemented");
        }

        /// Test-only: applies catalog product grants without going through store.
        /// Phase 2 real IAP path: store callback → ApplyGrants(product) after receipt validation.
        public void ApplyGrants(IapProductDef product)
        {
            if (product == null) return;
            if (product.GoldGrant > 0) EconomySim.Instance?.GrantGold(product.GoldGrant);
            if (product.GemGrant > 0)
            {
                // Phase 2: gem currency. Phase 1: log only.
                Debug.Log($"[MonetizationService] grant {product.GemGrant} gems (Phase 2 TBD)");
            }
            EventBus.RaiseIapCompleted(product.Sku, true);
            PurchaseCompleted?.Invoke(product.Sku, new Dictionary<string, object>
            {
                ["gem_grant"] = product.GemGrant,
                ["gold_grant"] = product.GoldGrant,
                ["no_ads"] = product.NoAdsUnlock,
            });
        }

        public void ShowRewardedAd(string placement)
        {
            Debug.LogWarning($"[MonetizationService] rewarded ad stub for {placement}");
            AdCompleted?.Invoke(placement, new Dictionary<string, object>());
        }

        public void ShowInterstitial(string placement)
            => Debug.LogWarning($"[MonetizationService] interstitial stub for {placement}");

        public void RestorePurchases() => Debug.LogWarning("[MonetizationService] restore stub");

        public bool HasActiveSubscription() => false;
    }
}

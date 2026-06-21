using System;
using System.Collections.Generic;
using AnimeEmpire.Core;
using UnityEngine;

namespace AnimeEmpire.Backend
{
    public class MonetizationService : MonoBehaviour
    {
        public static MonetizationService Instance { get; private set; }

        public event Action<string, Dictionary<string, object>> PurchaseCompleted;
        public event Action<string, string> PurchaseFailed;
        public event Action<string, Dictionary<string, object>> AdCompleted;
        public event Action<bool> SubscriptionChanged;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            Debug.Log("[MonetizationService] ready (stub)");
        }

        void OnDestroy() { if (Instance == this) Instance = null; }

        public void Purchase(string sku)
        {
            Debug.LogWarning($"[MonetizationService] purchase stub for sku={sku}");
            EventBus.RaiseIapCompleted(sku, false);
            PurchaseFailed?.Invoke(sku, "not_implemented");
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

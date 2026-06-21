using System.Collections;
using AnimeEmpire.Core;
using AnimeEmpire.Data;
using AnimeEmpire.Economy;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace AnimeEmpire.Tests.PlayMode
{
    public class EconomyIntegrationTests
    {
        [UnityTest]
        public IEnumerator ProductionLine_TickedByEconomySim_AccumulatesInventory()
        {
            // Bootstrap.prefab is spawned automatically via AppBootstrap on BeforeSceneLoad,
            // so EconomySim.Instance should exist by frame 1.
            yield return null;
            yield return null;
            Assert.That(EconomySim.Instance, Is.Not.Null, "Bootstrap.prefab missing — run Tools → Build Phase 1 Content");

            var wheat = ScriptableObject.CreateInstance<ResourceDef>();
            wheat.Id = "wheat"; wheat.BaseSellPrice = 1;

            var farm = ScriptableObject.CreateInstance<BuildingDef>();
            farm.Id = "wheat_farm";
            farm.Category = "generator";
            farm.BaseCycleSeconds = 0.2f;
            farm.OutputResource = wheat;
            farm.OutputAmount = 1;

            EconomySim.Instance.RegisterLineFromDef(farm);
            int startInv = EconomySim.Instance.GetInventory("wheat");
            // 1 second real time → 5 cycles @ 0.2s each → at least 4 emissions.
            yield return new WaitForSeconds(1.1f);
            int delta = EconomySim.Instance.GetInventory("wheat") - startInv;
            Assert.That(delta, Is.GreaterThanOrEqualTo(4));
        }

        [UnityTest]
        public IEnumerator SellInventory_RaisesCurrencyChanged()
        {
            yield return null;
            Assert.That(EconomySim.Instance, Is.Not.Null);

            var wheat = ScriptableObject.CreateInstance<ResourceDef>();
            wheat.Id = "wheat_test_sell"; wheat.BaseSellPrice = 5;

            string lastKind = null;
            int lastValue = -1;
            void Handler(string k, int v) { lastKind = k; lastValue = v; }
            EventBus.CurrencyChanged += Handler;
            try
            {
                EventBus.RaiseResourceProduced("test", "wheat_test_sell", 4);
                int gold = EconomySim.Instance.SellInventory(wheat);
                Assert.That(gold, Is.EqualTo(20));
                Assert.That(lastKind, Is.EqualTo("gold"));
                Assert.That(lastValue, Is.GreaterThanOrEqualTo(20));
            }
            finally
            {
                EventBus.CurrencyChanged -= Handler;
            }
            yield return null;
        }
    }
}

using AnimeEmpire.Core;
using AnimeEmpire.Data;
using AnimeEmpire.Economy;
using NUnit.Framework;
using UnityEngine;

namespace AnimeEmpire.Tests.EditMode
{
    public class EconomySimFlowTests
    {
        GameObject _host;
        EconomySim _sim;

        [SetUp]
        public void SetUp()
        {
            _host = new GameObject("TestEconomySim");
            _sim = _host.AddComponent<EconomySim>();
            // Awake runs on AddComponent in EditMode for MonoBehaviours.
        }

        [TearDown]
        public void TearDown()
        {
            if (_host != null) Object.DestroyImmediate(_host);
        }

        ResourceDef MakeResource(string id, int price)
        {
            var r = ScriptableObject.CreateInstance<ResourceDef>();
            r.Id = id; r.BaseSellPrice = price;
            return r;
        }

        [Test]
        public void Produced_Accumulates_Inventory()
        {
            EventBus.RaiseResourceProduced("wheat_farm", "wheat", 3);
            EventBus.RaiseResourceProduced("wheat_farm", "wheat", 2);
            Assert.That(_sim.GetInventory("wheat"), Is.EqualTo(5));
        }

        [Test]
        public void SellInventory_AddsGold_ZerosInventory()
        {
            var wheat = MakeResource("wheat", 1);
            EventBus.RaiseResourceProduced("wheat_farm", "wheat", 10);
            int gold = _sim.SellInventory(wheat);
            Assert.That(gold, Is.EqualTo(10));
            Assert.That(_sim.GetGold(), Is.EqualTo(10));
            Assert.That(_sim.GetInventory("wheat"), Is.EqualTo(0));
        }

        [Test]
        public void SpendGold_FailsWhenInsufficient()
        {
            Assert.That(_sim.SpendGold(50), Is.False);
            _sim.GrantGold(20);
            Assert.That(_sim.SpendGold(50), Is.False);
            Assert.That(_sim.GetGold(), Is.EqualTo(20));
        }

        [Test]
        public void SpendGold_SucceedsAndDeducts()
        {
            _sim.GrantGold(100);
            Assert.That(_sim.SpendGold(40), Is.True);
            Assert.That(_sim.GetGold(), Is.EqualTo(60));
        }

        [Test]
        public void SellEmpty_ReturnsZero()
        {
            var wheat = MakeResource("wheat", 5);
            int gold = _sim.SellInventory(wheat);
            Assert.That(gold, Is.EqualTo(0));
            Assert.That(_sim.GetGold(), Is.EqualTo(0));
        }

        [Test]
        public void SimulateOffline_CapsAt4Hours()
        {
            var result = _sim.SimulateOffline(10 * 3600);
            Assert.That(result.ElapsedEffective, Is.EqualTo(4 * 3600 * EconomySim.OfflineEfficiency).Within(0.01));
        }
    }
}

using AnimeEmpire.Core;
using AnimeEmpire.Data;
using AnimeEmpire.Economy;
using NUnit.Framework;
using UnityEngine;

namespace AnimeEmpire.Tests.EditMode
{
    public class SimulateOfflineTests
    {
        GameObject _host;
        EconomySim _sim;

        [SetUp]
        public void SetUp()
        {
            _host = new GameObject("TestEconomyOffline");
            _sim = _host.AddComponent<EconomySim>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_host != null) Object.DestroyImmediate(_host);
        }

        ProductionLine Register(float cycle, int amount = 1)
        {
            var res = ScriptableObject.CreateInstance<ResourceDef>();
            res.Id = "wheat"; res.BaseSellPrice = 1;
            var def = ScriptableObject.CreateInstance<BuildingDef>();
            def.Id = "wheat_farm"; def.Category = "generator";
            def.OutputResource = res; def.OutputAmount = amount;
            def.BaseCycleSeconds = cycle;
            return _sim.RegisterLineFromDef(def);
        }

        [Test]
        public void ZeroElapsed_ReturnsEmpty()
        {
            var r = _sim.SimulateOffline(0);
            Assert.That(r.Resources.Count, Is.EqualTo(0));
            Assert.That(r.ElapsedEffective, Is.EqualTo(0));
        }

        [Test]
        public void NegativeElapsed_TreatedAsZero()
        {
            var r = _sim.SimulateOffline(-100);
            Assert.That(r.Resources.Count, Is.EqualTo(0));
        }

        [Test]
        public void CapsAt4Hours()
        {
            var r = _sim.SimulateOffline(10 * 3600);
            Assert.That(r.ElapsedEffective, Is.EqualTo(EconomySim.OfflineCapSeconds * EconomySim.OfflineEfficiency).Within(0.01));
        }

        [Test]
        public void OneLine_CycleOne_ProducesEffectiveSeconds()
        {
            Register(cycle: 1f);
            var r = _sim.SimulateOffline(100);
            // effective = 100 * 0.5 = 50s. cycle=1s → 50 cycles → 50 units
            Assert.That(r.Resources["wheat"], Is.EqualTo(50));
            Assert.That(_sim.GetInventory("wheat"), Is.EqualTo(50));
        }

        [Test]
        public void NoLines_ProducesNothing()
        {
            var r = _sim.SimulateOffline(3600);
            Assert.That(r.Resources.Count, Is.EqualTo(0));
        }
    }
}

using System.Collections.Generic;
using AnimeEmpire.Core;
using AnimeEmpire.Data;
using AnimeEmpire.Economy;
using NUnit.Framework;
using UnityEngine;

namespace AnimeEmpire.Tests.EditMode
{
    public class ProductionLineTests
    {
        int _emissionCount;
        string _lastResource;

        ResourceDef MakeResource(string id, int price)
        {
            var r = ScriptableObject.CreateInstance<ResourceDef>();
            r.Id = id; r.BaseSellPrice = price;
            return r;
        }

        BuildingDef MakeBuilding(string id, float cycle, ResourceDef output)
        {
            var b = ScriptableObject.CreateInstance<BuildingDef>();
            b.Id = id; b.BaseCycleSeconds = cycle; b.OutputResource = output; b.OutputAmount = 1;
            return b;
        }

        ProductionLine MakeLine(BuildingDef building, int level, bool owned)
            => new() { BuildingDef = building, CurrentLevel = level, Owned = owned };

        [SetUp]
        public void SetUp()
        {
            _emissionCount = 0;
            _lastResource = null;
            EventBus.ResourceProduced += Handler;
        }

        [TearDown]
        public void TearDown()
        {
            EventBus.ResourceProduced -= Handler;
        }

        void Handler(string buildingId, string resourceId, int amount)
        {
            _emissionCount++;
            _lastResource = resourceId;
        }

        [Test]
        public void OneFullCycle_EmitsOnce()
        {
            var wheat = MakeResource("wheat", 1);
            var farm = MakeBuilding("wheat_farm", 1f, wheat);
            var line = MakeLine(farm, 1, true);
            line.Tick(1f, null);
            Assert.That(_emissionCount, Is.EqualTo(1));
            Assert.That(_lastResource, Is.EqualTo("wheat"));
        }

        [Test]
        public void Unowned_DoesNotEmit()
        {
            var wheat = MakeResource("wheat", 1);
            var farm = MakeBuilding("wheat_farm", 1f, wheat);
            var line = MakeLine(farm, 1, false);
            line.Tick(5f, null);
            Assert.That(_emissionCount, Is.EqualTo(0));
        }

        [Test]
        public void SpeedMultiplier_DoublesOutput()
        {
            var wheat = MakeResource("wheat", 1);
            var farm = MakeBuilding("wheat_farm", 1f, wheat);
            var line = MakeLine(farm, 1, true);
            line.Tick(1f, new Dictionary<string, float> { { "speed", 2f } });
            Assert.That(_emissionCount, Is.EqualTo(2));
        }

        [Test]
        public void ProgressAccumulates_AcrossTicks()
        {
            var wheat = MakeResource("wheat", 1);
            var farm = MakeBuilding("wheat_farm", 1f, wheat);
            var line = MakeLine(farm, 1, true);
            line.Tick(0.5f, null);
            Assert.That(_emissionCount, Is.EqualTo(0), "premature emission after 0.5s");
            line.Tick(0.5f, null);
            Assert.That(_emissionCount, Is.EqualTo(1));
        }

        [Test]
        public void NullOutput_DoesNotCrash()
        {
            var farm = MakeBuilding("market", 0f, null);
            var line = MakeLine(farm, 1, true);
            line.Tick(1f, null);
            Assert.That(_emissionCount, Is.EqualTo(0));
        }
    }
}

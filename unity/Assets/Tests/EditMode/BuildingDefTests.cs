using AnimeEmpire.Data;
using AnimeEmpire.Economy;
using NUnit.Framework;
using UnityEngine;

namespace AnimeEmpire.Tests.EditMode
{
    public class BuildingDefTests
    {
        static ProductionLine MakeLine(float cycle, int baseCost, float growth, int level)
        {
            var wheat = ScriptableObject.CreateInstance<ResourceDef>();
            wheat.Id = "wheat"; wheat.BaseSellPrice = 1;

            var farm = ScriptableObject.CreateInstance<BuildingDef>();
            farm.BaseCycleSeconds = cycle;
            farm.BaseCostGold = baseCost;
            farm.CostGrowth = growth;
            farm.OutputResource = wheat;
            farm.OutputAmount = 1;

            return new ProductionLine { BuildingDef = farm, CurrentLevel = level };
        }

        [Test]
        public void Cost_AtLevel1_IsBase()
        {
            var line = MakeLine(1f, 100, 1.12f, 1);
            Assert.That(line.GetUpgradeCost(), Is.EqualTo(100));
        }

        [Test]
        public void Cost_AtLevel5_Is157()
        {
            var line = MakeLine(1f, 100, 1.12f, 5);
            Assert.That(line.GetUpgradeCost(), Is.EqualTo(157));
        }

        [Test]
        public void Cost_AtLevel10_Is277()
        {
            var line = MakeLine(1f, 100, 1.12f, 10);
            Assert.That(line.GetUpgradeCost(), Is.EqualTo(277));
        }

        [Test]
        public void IncomeRate_AtLevel1_IsOne()
        {
            var line = MakeLine(1f, 100, 1.12f, 1);
            Assert.That(line.GetIncomeRate(), Is.EqualTo(1f).Within(0.001f));
        }

        [Test]
        public void IncomeRate_GrowsWithLevel()
        {
            var l1 = MakeLine(1f, 100, 1.12f, 1);
            var l5 = MakeLine(1f, 100, 1.12f, 5);
            Assert.That(l5.GetIncomeRate(), Is.GreaterThan(l1.GetIncomeRate()));
        }
    }
}

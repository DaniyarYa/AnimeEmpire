using System.Collections.Generic;
using AnimeEmpire.Backend;
using AnimeEmpire.Data;
using AnimeEmpire.Economy;
using NUnit.Framework;
using UnityEngine;

namespace AnimeEmpire.Tests.EditMode
{
    public class NotificationServiceTests
    {
        ProductionLine MakeLine(float cycle, bool owned, int level = 1)
        {
            var def = ScriptableObject.CreateInstance<BuildingDef>();
            def.Id = "test_farm";
            def.Category = "generator";
            def.BaseCycleSeconds = cycle;
            def.OutputAmount = 1;
            var res = ScriptableObject.CreateInstance<ResourceDef>();
            res.Id = "wheat";
            def.OutputResource = res;
            return new ProductionLine { BuildingDef = def, CurrentLevel = level, Owned = owned };
        }

        [Test]
        public void Compute_NullList_ReturnsZero()
        {
            Assert.That(NotificationService.ComputeNextHarvestSeconds(null), Is.EqualTo(0));
        }

        [Test]
        public void Compute_EmptyList_ReturnsZero()
        {
            Assert.That(NotificationService.ComputeNextHarvestSeconds(new List<ProductionLine>()), Is.EqualTo(0));
        }

        [Test]
        public void Compute_UnownedLine_Ignored()
        {
            var lines = new List<ProductionLine> { MakeLine(60f, owned: false) };
            Assert.That(NotificationService.ComputeNextHarvestSeconds(lines), Is.EqualTo(0));
        }

        [Test]
        public void Compute_OneOwnedLine_ReturnsCycleAtFreshProgress()
        {
            var lines = new List<ProductionLine> { MakeLine(120f, owned: true) };
            int s = NotificationService.ComputeNextHarvestSeconds(lines);
            Assert.That(s, Is.InRange(119, 121));
        }

        [Test]
        public void Compute_MultipleLines_PicksShortest()
        {
            var lines = new List<ProductionLine>
            {
                MakeLine(300f, owned: true),
                MakeLine(60f,  owned: true),
                MakeLine(900f, owned: true),
            };
            int s = NotificationService.ComputeNextHarvestSeconds(lines);
            Assert.That(s, Is.InRange(59, 61));
        }

        [Test]
        public void Compute_CycleDecayPerLevel_ReducesDelay()
        {
            var l1 = new List<ProductionLine> { MakeLine(100f, owned: true, level: 1) };
            var l10 = new List<ProductionLine> { MakeLine(100f, owned: true, level: 10) };
            int s1 = NotificationService.ComputeNextHarvestSeconds(l1);
            int s10 = NotificationService.ComputeNextHarvestSeconds(l10);
            Assert.That(s10, Is.LessThan(s1));
        }
    }
}

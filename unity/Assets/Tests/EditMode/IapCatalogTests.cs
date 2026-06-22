using System.Collections.Generic;
using AnimeEmpire.Data;
using NUnit.Framework;
using UnityEngine;

namespace AnimeEmpire.Tests.EditMode
{
    public class IapCatalogTests
    {
        IapProductDef Make(string id, string sku, int gems = 0, bool noAds = false)
        {
            var p = ScriptableObject.CreateInstance<IapProductDef>();
            p.Id = id; p.Sku = sku; p.GemGrant = gems; p.NoAdsUnlock = noAds;
            return p;
        }

        [Test]
        public void FindBySku_ReturnsMatching()
        {
            var cat = ScriptableObject.CreateInstance<IapCatalog>();
            var gems100 = Make("gems_100", "com.x.gems_100", 100);
            var noAds = Make("no_ads", "com.x.no_ads", 0, true);
            cat.Products = new List<IapProductDef> { gems100, noAds };
            Assert.That(cat.FindBySku("com.x.no_ads"), Is.SameAs(noAds));
            Assert.That(cat.FindBySku("missing"), Is.Null);
        }

        [Test]
        public void FindById_IgnoresMissing()
        {
            var cat = ScriptableObject.CreateInstance<IapCatalog>();
            cat.Products = new List<IapProductDef> { Make("gems_100", "x") };
            Assert.That(cat.FindById("gems_100").Id, Is.EqualTo("gems_100"));
            Assert.That(cat.FindById("unknown"), Is.Null);
        }

        [Test]
        public void FindBySku_NullEntries_DoesNotCrash()
        {
            var cat = ScriptableObject.CreateInstance<IapCatalog>();
            cat.Products = new List<IapProductDef> { null, Make("x", "y") };
            Assert.That(cat.FindBySku("y"), Is.Not.Null);
        }
    }
}

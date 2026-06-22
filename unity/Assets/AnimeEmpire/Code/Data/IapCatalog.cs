using System.Collections.Generic;
using UnityEngine;

namespace AnimeEmpire.Data
{
    [CreateAssetMenu(fileName = "IapCatalog", menuName = "Anime Empire/IAP Catalog")]
    public class IapCatalog : ScriptableObject
    {
        public List<IapProductDef> Products = new();

        public IapProductDef FindBySku(string sku)
        {
            for (int i = 0; i < Products.Count; i++)
                if (Products[i] != null && Products[i].Sku == sku) return Products[i];
            return null;
        }

        public IapProductDef FindById(string id)
        {
            for (int i = 0; i < Products.Count; i++)
                if (Products[i] != null && Products[i].Id == id) return Products[i];
            return null;
        }
    }
}

using System.Collections.Generic;
using AnimeEmpire.Core;
using AnimeEmpire.Data;
using UnityEngine;

namespace AnimeEmpire.Economy
{
    public class ProductionLine
    {
        public const float CycleDecayPerLevel = 0.97f;

        public BuildingDef BuildingDef;
        public int CurrentLevel = 1;
        public bool Owned;

        float _progress;

        public void Tick(float dt, IReadOnlyDictionary<string, float> modifiers)
        {
            if (!Owned || BuildingDef == null || BuildingDef.OutputResource == null) return;
            float speedMult = modifiers != null && modifiers.TryGetValue("speed", out var s) ? s : 1f;
            float cycle = BuildingDef.BaseCycleSeconds * Mathf.Pow(CycleDecayPerLevel, CurrentLevel - 1);
            cycle /= Mathf.Max(speedMult, 0.0001f);
            _progress += dt / cycle;
            while (_progress >= 1f)
            {
                _progress -= 1f;
                EventBus.RaiseResourceProduced(BuildingDef.Id, BuildingDef.OutputResource.Id, BuildingDef.OutputAmount);
            }
        }

        public int GetUpgradeCost()
        {
            if (BuildingDef == null) return 0;
            return Mathf.RoundToInt(BuildingDef.BaseCostGold * Mathf.Pow(BuildingDef.CostGrowth, CurrentLevel - 1));
        }

        public float GetIncomeRate()
        {
            if (BuildingDef == null || BuildingDef.OutputResource == null) return 0f;
            float cycle = BuildingDef.BaseCycleSeconds * Mathf.Pow(CycleDecayPerLevel, CurrentLevel - 1);
            return BuildingDef.OutputAmount * BuildingDef.OutputResource.BaseSellPrice / Mathf.Max(cycle, 0.0001f);
        }

        public float GetProgress() => _progress;
    }
}

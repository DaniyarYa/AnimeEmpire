using System.Collections.Generic;
using UnityEngine;

namespace AnimeEmpire.Data
{
    [CreateAssetMenu(fileName = "TutorialFlow", menuName = "Anime Empire/Tutorial Flow")]
    public class TutorialFlow : ScriptableObject
    {
        public List<TutorialStep> Steps = new();

        public TutorialStep GetStep(int index)
            => (index >= 0 && index < Steps.Count) ? Steps[index] : null;

        public TutorialStep FindById(string id)
        {
            for (int i = 0; i < Steps.Count; i++)
                if (Steps[i] != null && Steps[i].Id == id) return Steps[i];
            return null;
        }

        public int Count => Steps.Count;
    }
}

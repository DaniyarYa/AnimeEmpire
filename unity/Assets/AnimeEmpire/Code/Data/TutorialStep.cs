using UnityEngine;

namespace AnimeEmpire.Data
{
    public enum TutorialAdvance
    {
        TapAnywhere,
        ResourceProduced,
        ResourceSold,
        BuildingClicked,
    }

    [CreateAssetMenu(fileName = "TutorialStep", menuName = "Anime Empire/Tutorial Step")]
    public class TutorialStep : ScriptableObject
    {
        public string Id = "";
        public string MessageKey = "";

        [Tooltip("Tag of the GameObject to highlight in the world. Empty = no highlight.")]
        public string TargetTag = "";

        public TutorialAdvance Advance = TutorialAdvance.TapAnywhere;

        [Tooltip("For BuildingClicked: building Id to expect. Empty = any building.")]
        public string ExpectedBuildingId = "";

        [Tooltip("For ResourceProduced/Sold: resource Id to expect. Empty = any.")]
        public string ExpectedResourceId = "";
    }
}

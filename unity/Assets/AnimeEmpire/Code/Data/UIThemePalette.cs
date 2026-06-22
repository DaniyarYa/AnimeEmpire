using UnityEngine;

namespace AnimeEmpire.Data
{
    /// Color palette mirroring godot/themes/main.theme.tres. Authored once via
    /// ContentBuilder, consumed by ContentBuilder to tint authored buttons/panels
    /// + at runtime by any custom button skin.
    [CreateAssetMenu(fileName = "UIThemePalette", menuName = "Anime Empire/UI Theme Palette")]
    public class UIThemePalette : ScriptableObject
    {
        [Header("Button (mustard yellow per godot theme)")]
        public Color ButtonNormal = new(1f, 0.722f, 0.302f, 1f);
        public Color ButtonHover = new(1f, 0.78f, 0.4f, 1f);
        public Color ButtonPressed = new(0.85f, 0.6f, 0.2f, 1f);
        public Color ButtonDisabled = new(0.612f, 0.576f, 0.596f, 1f);

        [Header("Text")]
        public Color TextDefault = new(0.176f, 0.165f, 0.18f, 1f);
        public Color TextPressed = new(1f, 1f, 1f, 1f);
        public Color TextDisabled = new(1f, 1f, 1f, 0.6f);

        [Header("Panel (cream)")]
        public Color PanelBg = new(1f, 0.957f, 0.878f, 1f);

        [Header("Font sizes")]
        public int FontSizeDefault = 16;
        public int FontSizeButton = 16;
    }
}

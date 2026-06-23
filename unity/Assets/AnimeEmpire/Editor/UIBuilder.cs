using AnimeEmpire.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnimeEmpire.Editor
{
    /// Palette-applied uGUI factory. ContentBuilder calls these to keep
    /// styling centralized — modify UIBuilder + UIThemePalette to restyle
    /// every authored panel/button/text/slider at once.
    public static class UIBuilder
    {
        public static GameObject Panel(Transform parent, string name, UIThemePalette p)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = p != null ? p.PanelBg : new Color(1f, 0.957f, 0.878f, 1f);
            var layout = go.AddComponent<VerticalLayoutGroup>();
            int pad = p != null ? p.PanelPadding : 24;
            layout.padding = new RectOffset(pad, pad, pad, pad);
            layout.spacing = p != null ? p.Spacing : 16;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            return go;
        }

        public static GameObject HorizontalRow(Transform parent, string name, UIThemePalette p)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>();
            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = p != null ? p.Spacing : 16;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            var le = go.AddComponent<LayoutElement>();
            le.minHeight = p != null ? p.ButtonMinHeight : 72;
            return go;
        }

        public static TMP_Text Text(Transform parent, string name, string content, int size, UIThemePalette p, TextAlignmentOptions align = TextAlignmentOptions.Center)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>();
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = content;
            tmp.fontSize = size;
            tmp.color = p != null ? p.TextDefault : Color.white;
            tmp.alignment = align;
            tmp.enableWordWrapping = true;
            var le = go.AddComponent<LayoutElement>();
            le.minHeight = size + 8;
            return tmp;
        }

        public static GameObject Button(Transform parent, string name, string label, UIThemePalette p)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>();
            var img = go.AddComponent<Image>();
            img.color = p != null ? p.ButtonNormal : new Color(0.2f, 0.2f, 0.25f, 0.9f);
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            if (p != null)
            {
                var colors = btn.colors;
                colors.normalColor = p.ButtonNormal;
                colors.highlightedColor = p.ButtonHover;
                colors.pressedColor = p.ButtonPressed;
                colors.disabledColor = p.ButtonDisabled;
                btn.colors = colors;
            }
            var le = go.AddComponent<LayoutElement>();
            le.minHeight = p != null ? p.ButtonMinHeight : 72;
            le.preferredHeight = p != null ? p.ButtonMinHeight : 72;

            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(go.transform, false);
            var lrt = labelGO.AddComponent<RectTransform>();
            lrt.anchorMin = new Vector2(0.08f, 0); lrt.anchorMax = new Vector2(0.92f, 1);
            lrt.offsetMin = Vector2.zero; lrt.offsetMax = Vector2.zero;
            var tmp = labelGO.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = p != null ? p.FontSizeButton : 32;
            tmp.color = p != null ? p.TextDefault : Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableWordWrapping = false;
            return go;
        }

        public static Slider Slider(Transform parent, string name, UIThemePalette p)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>();
            var bg = go.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.18f);
            var slider = go.AddComponent<Slider>();
            slider.minValue = 0f; slider.maxValue = 1f; slider.value = 0.5f;
            var le = go.AddComponent<LayoutElement>();
            le.minHeight = 40;

            var fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(go.transform, false);
            var fillAreaRT = fillArea.AddComponent<RectTransform>();
            fillAreaRT.anchorMin = new Vector2(0, 0.25f); fillAreaRT.anchorMax = new Vector2(1, 0.75f);
            fillAreaRT.offsetMin = new Vector2(8, 0); fillAreaRT.offsetMax = new Vector2(-8, 0);

            var fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            var fRT = fill.AddComponent<RectTransform>();
            fRT.anchorMin = Vector2.zero; fRT.anchorMax = Vector2.one;
            fRT.offsetMin = Vector2.zero; fRT.offsetMax = Vector2.zero;
            var fImg = fill.AddComponent<Image>();
            fImg.color = p != null ? p.ButtonNormal : new Color(0.4f, 0.65f, 1f, 0.95f);
            slider.fillRect = fRT;
            slider.targetGraphic = bg;
            return slider;
        }

        public static TMP_Dropdown Dropdown(Transform parent, string name, UIThemePalette p)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>();
            var bg = go.AddComponent<Image>();
            bg.color = p != null ? p.ButtonNormal : new Color(0.2f, 0.2f, 0.25f, 0.95f);
            var le = go.AddComponent<LayoutElement>();
            le.minHeight = 56;
            var dd = go.AddComponent<TMP_Dropdown>();
            dd.targetGraphic = bg;
            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(go.transform, false);
            var lrt = labelGO.AddComponent<RectTransform>();
            lrt.anchorMin = new Vector2(0.05f, 0); lrt.anchorMax = new Vector2(0.95f, 1);
            lrt.offsetMin = Vector2.zero; lrt.offsetMax = Vector2.zero;
            var tmp = labelGO.AddComponent<TextMeshProUGUI>();
            tmp.fontSize = p != null ? p.FontSizeDefault : 24;
            tmp.color = p != null ? p.TextDefault : Color.white;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            dd.captionText = tmp;
            return dd;
        }
    }
}

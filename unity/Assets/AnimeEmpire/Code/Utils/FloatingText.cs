using System.Collections;
using TMPro;
using UnityEngine;

namespace AnimeEmpire.Utils
{
    public class FloatingText : MonoBehaviour
    {
        public const float RiseDistance = 1.2f;
        public const float Duration = 1.0f;

        TextMeshPro _label;

        public static FloatingText Spawn(Transform parent, Vector3 worldPos, string text, Color color)
        {
            var go = new GameObject("FloatingText");
            if (parent != null) go.transform.SetParent(parent, false);
            go.transform.position = worldPos;

            var tmp = go.AddComponent<TextMeshPro>();
            tmp.text = text;
            tmp.color = color;
            tmp.fontSize = 6;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.outlineWidth = 0.2f;
            tmp.outlineColor = new Color32(0, 0, 0, 200);

            go.AddComponent<Billboard>();
            var ft = go.AddComponent<FloatingText>();
            ft._label = tmp;
            ft.StartCoroutine(ft.AnimateRoutine());
            return ft;
        }

        IEnumerator AnimateRoutine()
        {
            var start = transform.position;
            var end = start + Vector3.up * RiseDistance;
            float t = 0f;
            var startColor = _label.color;
            while (t < Duration)
            {
                t += Time.deltaTime;
                float u = Mathf.Clamp01(t / Duration);
                float ease = 1f - (1f - u) * (1f - u);
                transform.position = Vector3.Lerp(start, end, ease);
                float alphaT = Mathf.Clamp01((t - Duration * 0.4f) / (Duration * 0.6f));
                _label.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, 0f, alphaT));
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}

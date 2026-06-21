using UnityEngine;

namespace AnimeEmpire.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaFitter : MonoBehaviour
    {
        RectTransform _rt;
        Rect _lastSafeArea;
        Vector2Int _lastResolution;
        ScreenOrientation _lastOrientation;

        void Awake() => _rt = GetComponent<RectTransform>();

        void OnEnable() => Apply();

        void Update()
        {
            if (_lastSafeArea != Screen.safeArea
                || _lastResolution.x != Screen.width
                || _lastResolution.y != Screen.height
                || _lastOrientation != Screen.orientation)
            {
                Apply();
            }
        }

        void Apply()
        {
            if (_rt == null) return;
            var safe = Screen.safeArea;
            var anchorMin = safe.position;
            var anchorMax = safe.position + safe.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            _rt.anchorMin = anchorMin;
            _rt.anchorMax = anchorMax;
            _rt.offsetMin = Vector2.zero;
            _rt.offsetMax = Vector2.zero;
            _lastSafeArea = safe;
            _lastResolution = new Vector2Int(Screen.width, Screen.height);
            _lastOrientation = Screen.orientation;
        }
    }
}

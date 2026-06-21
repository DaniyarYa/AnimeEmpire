using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AnimeEmpire.Core
{
    public class SceneRouter : MonoBehaviour
    {
        public const string SceneBoot = "Boot";
        public const string SceneWorld = "World";
        public const float FadeDuration = 0.2f;

        public static SceneRouter Instance { get; private set; }

        readonly List<string> _stack = new();
        Image _fadeImage;
        Canvas _fadeCanvas;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            SetupFadeOverlay();
            Debug.Log("[SceneRouter] ready");
        }

        void OnDestroy() { if (Instance == this) Instance = null; }

        void SetupFadeOverlay()
        {
            var go = new GameObject("SceneRouter_FadeCanvas");
            DontDestroyOnLoad(go);
            _fadeCanvas = go.AddComponent<Canvas>();
            _fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _fadeCanvas.sortingOrder = 32760;
            go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();

            var imgGo = new GameObject("Fade");
            imgGo.transform.SetParent(go.transform, false);
            _fadeImage = imgGo.AddComponent<Image>();
            _fadeImage.color = new Color(0, 0, 0, 0);
            _fadeImage.raycastTarget = false;
            var rt = _fadeImage.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        public void Push(string scene)
        {
            _stack.Add(scene);
            StartCoroutine(GoTo(scene));
        }

        public void Replace(string scene)
        {
            if (_stack.Count > 0) _stack[^1] = scene;
            else _stack.Add(scene);
            StartCoroutine(GoTo(scene));
        }

        public void Pop()
        {
            if (_stack.Count <= 1) return;
            _stack.RemoveAt(_stack.Count - 1);
            StartCoroutine(GoTo(_stack[^1]));
        }

        IEnumerator GoTo(string scene)
        {
            string prev = GameState.Instance != null ? GameState.Instance.CurrentScreen : "";
            yield return FadeTo(1f);
            var op = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
            while (op != null && !op.isDone) yield return null;
            if (GameState.Instance != null) GameState.Instance.CurrentScreen = scene;
            EventBus.RaiseScreenChanged(prev, scene);
            yield return FadeTo(0f);
        }

        IEnumerator FadeTo(float targetAlpha)
        {
            if (_fadeImage == null) yield break;
            float start = _fadeImage.color.a;
            float t = 0f;
            while (t < FadeDuration)
            {
                t += Time.unscaledDeltaTime;
                float a = Mathf.Lerp(start, targetAlpha, Mathf.Clamp01(t / FadeDuration));
                var c = _fadeImage.color; c.a = a; _fadeImage.color = c;
                yield return null;
            }
            var col = _fadeImage.color; col.a = targetAlpha; _fadeImage.color = col;
        }
    }
}

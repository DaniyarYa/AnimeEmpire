#if UNITY_EDITOR || DEVELOPMENT_BUILD
using AnimeEmpire.Economy;
using AnimeEmpire.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AnimeEmpire.Utils
{
    public class DebugOverlay : MonoBehaviour
    {
        const float SmoothFactor = 0.1f;
        bool _shown;
        float _smoothDt;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoSpawn()
        {
            if (FindFirstObjectByType<DebugOverlay>() != null) return;
            var go = new GameObject("DebugOverlay");
            go.AddComponent<DebugOverlay>();
            DontDestroyOnLoad(go);
        }

        void Update()
        {
            _smoothDt += (Time.unscaledDeltaTime - _smoothDt) * SmoothFactor;
            var kb = Keyboard.current;
            if (kb != null && kb.f3Key.wasPressedThisFrame) _shown = !_shown;
        }

        void OnGUI()
        {
            if (!_shown) return;
            float fps = _smoothDt > 0f ? 1f / _smoothDt : 0f;
            var area = new Rect(10, 10, 360, 200);
            GUI.color = new Color(1, 1, 1, 0.95f);
            GUI.Box(area, "Debug (F3)");
            GUILayout.BeginArea(new Rect(area.x + 8, area.y + 22, area.width - 16, area.height - 30));
            GUILayout.Label($"FPS: {fps:0.0}");
            GUILayout.Label($"Gold: {(EconomySim.Instance != null ? EconomySim.Instance.GetGold() : 0)}");
            GUILayout.Label($"Buildings: {BuildingRegistry.All.Count}");
            GUILayout.Label($"NPCs: {NpcRegistry.All.Count}");
            GUILayout.Label($"Memory: {System.GC.GetTotalMemory(false) / (1024 * 1024)} MB");
            GUILayout.Label($"Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
            GUILayout.EndArea();
        }
    }
}
#endif

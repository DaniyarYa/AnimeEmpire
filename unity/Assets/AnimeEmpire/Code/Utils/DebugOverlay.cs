#if UNITY_EDITOR || DEVELOPMENT_BUILD
using AnimeEmpire.Economy;
using AnimeEmpire.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AnimeEmpire.Utils
{
    public class DebugOverlay : MonoBehaviour
    {
        const float SmoothFactor = 0.1f;
        const float Budget60Fps = 0.0166f;
        const float Budget30Fps = 0.0333f;

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
            var area = new Rect(10, 10, 380, 260);
            GUI.color = new Color(1, 1, 1, 0.95f);
            GUI.Box(area, "Debug (F3)");
            GUILayout.BeginArea(new Rect(area.x + 8, area.y + 22, area.width - 16, area.height - 30));

            var fpsColor = _smoothDt <= Budget60Fps ? Color.green
                         : _smoothDt <= Budget30Fps ? Color.yellow
                         : Color.red;
            var prev = GUI.color;
            GUI.color = fpsColor;
            GUILayout.Label($"FPS: {fps:0.0}  ({_smoothDt * 1000f:0.0} ms)");
            GUI.color = prev;

            GUILayout.Label($"Gold: {(EconomySim.Instance != null ? EconomySim.Instance.GetGold() : 0)}");
            GUILayout.Label($"Buildings: {BuildingRegistry.All.Count}");
            GUILayout.Label($"NPCs: {NpcRegistry.All.Count}");
            GUILayout.Label($"Memory: {System.GC.GetTotalMemory(false) / (1024 * 1024)} MB");
            GUILayout.Label($"Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
#if UNITY_EDITOR
            // UnityStats API changed in Unity 6 — `batches` removed. Use drawCalls + tris + setPass.
            GUILayout.Label($"DrawCalls: {UnityStats.drawCalls}  Tris: {UnityStats.triangles}");
            GUILayout.Label($"SetPass: {UnityStats.setPassCalls}  Verts: {UnityStats.vertices}");
#endif
            GUILayout.EndArea();
        }
    }
}
#endif

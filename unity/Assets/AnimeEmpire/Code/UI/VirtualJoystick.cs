using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

namespace AnimeEmpire.UI
{
    public class VirtualJoystick : MonoBehaviour
    {
        public event Action<Vector2> DirectionChanged;
        public event Action TouchStarted;
        public event Action TouchEnded;

        public float MaxRadius = 80f;
        [Range(0f, 1f)] public float DeadZonePct = 0.1f;
        public Vector2 RestingPosition = new(160f, 200f);
        [Range(0f, 1f)] public float RestingAlpha = 0.35f;
        [Range(0f, 1f)] public float ActiveAlpha = 0.85f;
        [Range(0f, 1f)] public float ActiveZoneWidthPct = 0.5f;

        [SerializeField] RectTransform _background;
        [SerializeField] RectTransform _knob;
        [SerializeField] CanvasGroup _backgroundCanvasGroup;
        [SerializeField] Canvas _canvas;

        bool _active;
        int _touchFingerId = -1;
        Vector2 _origin;
        Vector2 _currentDirection;

        void OnEnable()
        {
            EnhancedTouchSupport.Enable();
            ShowAtRest();
        }

        void OnDisable() => EnhancedTouchSupport.Disable();

        void Update() => PollInput();

        void ShowAtRest()
        {
            if (_background == null) return;
            var size = new Vector2(Screen.width, Screen.height);
            _origin = new Vector2(RestingPosition.x, RestingPosition.y);
            _background.position = ScreenToCanvas(_origin);
            if (_knob != null) _knob.anchoredPosition = Vector2.zero;
            if (_backgroundCanvasGroup != null) _backgroundCanvasGroup.alpha = RestingAlpha;
        }

        Vector3 ScreenToCanvas(Vector2 screen)
        {
            if (_canvas == null) return new Vector3(screen.x, screen.y, 0);
            return new Vector3(screen.x, screen.y, 0);
        }

        void PollInput()
        {
            var touches = ETouch.Touch.activeTouches;
            if (touches.Count > 0)
            {
                if (!_active)
                {
                    var t = touches[0];
                    if (InActiveZone(t.screenPosition))
                        Start(t.screenPosition, t.finger.index);
                }
                else
                {
                    foreach (var t in touches)
                    {
                        if (t.finger.index == _touchFingerId)
                        {
                            if (t.phase == UnityEngine.InputSystem.TouchPhase.Ended || t.phase == UnityEngine.InputSystem.TouchPhase.Canceled) Stop();
                            else UpdateKnob(t.screenPosition);
                            return;
                        }
                    }
                    Stop();
                }
                return;
            }

            // Mouse fallback (editor / desktop)
            var mouse = UnityEngine.InputSystem.Mouse.current;
            if (mouse == null) return;
            if (mouse.leftButton.wasPressedThisFrame && !_active && InActiveZone(mouse.position.ReadValue()))
                Start(mouse.position.ReadValue(), -1);
            else if (mouse.leftButton.isPressed && _active && _touchFingerId == -1)
                UpdateKnob(mouse.position.ReadValue());
            else if (mouse.leftButton.wasReleasedThisFrame && _active && _touchFingerId == -1)
                Stop();
        }

        bool InActiveZone(Vector2 screenPos) => screenPos.x < Screen.width * ActiveZoneWidthPct;

        void Start(Vector2 at, int fingerId)
        {
            _active = true;
            _touchFingerId = fingerId;
            _origin = at;
            if (_background != null) _background.position = ScreenToCanvas(_origin);
            if (_knob != null) _knob.anchoredPosition = Vector2.zero;
            if (_backgroundCanvasGroup != null) _backgroundCanvasGroup.alpha = ActiveAlpha;
            TouchStarted?.Invoke();
        }

        void Stop()
        {
            _active = false;
            _touchFingerId = -1;
            _currentDirection = Vector2.zero;
            DirectionChanged?.Invoke(_currentDirection);
            ShowAtRest();
            TouchEnded?.Invoke();
        }

        void UpdateKnob(Vector2 at)
        {
            var offset = at - _origin;
            if (offset.magnitude > MaxRadius) offset = offset.normalized * MaxRadius;
            if (_knob != null) _knob.anchoredPosition = offset;
            var normalized = offset / MaxRadius;
            if (normalized.magnitude < DeadZonePct) normalized = Vector2.zero;
            else
            {
                float scale = (normalized.magnitude - DeadZonePct) / (1f - DeadZonePct);
                normalized = normalized.normalized * scale;
            }
            if (normalized != _currentDirection)
            {
                _currentDirection = normalized;
                DirectionChanged?.Invoke(_currentDirection);
            }
        }

        public Vector2 GetDirection() => _currentDirection;
        public bool IsActive => _active;
    }
}

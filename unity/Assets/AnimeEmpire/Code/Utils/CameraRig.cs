using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

namespace AnimeEmpire.Utils
{
    public class CameraRig : MonoBehaviour
    {
        public const float PinchSensitivity = 0.01f;
        public const float SmoothingBase = 30f;

        public Transform FollowTarget;
        [Range(0f, 0.99f)] public float FollowSmoothing = 0.15f;
        public Vector3 FollowOffset = new(0f, 8f, -12f);
        public float MinZoomDistance = 6f;
        public float MaxZoomDistance = 30f;
        public float FreeModeTimeout = 3f;

        float _freeModeTimer;
        float _lastPinchDistance;

        void OnEnable() => EnhancedTouchSupport.Enable();
        void OnDisable() => EnhancedTouchSupport.Disable();

        void Update()
        {
            if (FollowTarget == null) return;
            float dt = Time.deltaTime;
            if (_freeModeTimer > 0f) _freeModeTimer -= dt;

            Vector3 goal = FollowTarget.position + FollowOffset;
            float t = 1f - Mathf.Pow(FollowSmoothing, dt * SmoothingBase);
            transform.position = Vector3.Lerp(transform.position, goal, t);
            transform.LookAt(FollowTarget.position, Vector3.up);

            HandlePinch();
            HandleScrollZoom();
        }

        void HandlePinch()
        {
            var touches = ETouch.Touch.activeTouches;
            if (touches.Count < 2) { _lastPinchDistance = 0f; return; }
            float d = Vector2.Distance(touches[0].screenPosition, touches[1].screenPosition);
            if (_lastPinchDistance > 0f)
            {
                float delta = _lastPinchDistance - d;
                ZoomBy(delta * PinchSensitivity);
            }
            _lastPinchDistance = d;
            _freeModeTimer = FreeModeTimeout;
        }

        void HandleScrollZoom()
        {
            var mouse = UnityEngine.InputSystem.Mouse.current;
            if (mouse == null) return;
            float scroll = mouse.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) > 0.01f) ZoomBy(-scroll * 0.01f);
        }

        void ZoomBy(float amount)
        {
            Vector3 dir = FollowOffset.normalized;
            float dist = FollowOffset.magnitude;
            float newDist = Mathf.Clamp(dist + amount, MinZoomDistance, MaxZoomDistance);
            FollowOffset = dir * newDist;
        }
    }
}

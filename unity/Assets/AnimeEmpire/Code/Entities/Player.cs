using UnityEngine;

namespace AnimeEmpire.Entities
{
    [RequireComponent(typeof(CharacterController))]
    public class Player : MonoBehaviour
    {
        public const float Speed = 4f;
        public const float Accel = 18f;
        public const float RotationSpeed = 12f;

        [SerializeField] PlayerAnimationController _animController;

        CharacterController _cc;
        Vector2 _input;
        Vector3 _velocity;

        void Awake()
        {
            _cc = GetComponent<CharacterController>();
            if (_animController == null) _animController = GetComponentInChildren<PlayerAnimationController>();
        }

        // Joystick input: x=right, y=up-on-pad. We map y → world +Z (forward).
        // Unity forward is +Z, so up-on-pad pushes player along +Z. Sign differs from Godot
        // intentionally — Unity convention.
        public void SetMovementDirection(Vector2 dir) => _input = dir;

        void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            var target = new Vector3(_input.x, 0f, _input.y) * Speed;
            _velocity = Vector3.Lerp(_velocity, target, Mathf.Clamp01(Accel * dt));
            _cc.Move((_velocity + Physics.gravity) * dt);

            var horiz = new Vector3(_velocity.x, 0f, _velocity.z);
            if (_input.sqrMagnitude > 0.0001f && horiz.sqrMagnitude > 0.0001f)
            {
                var targetRot = Quaternion.LookRotation(horiz.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Mathf.Clamp01(RotationSpeed * dt));
            }

            if (_animController != null) _animController.UpdateSpeed(horiz.magnitude / Speed);
        }
    }
}

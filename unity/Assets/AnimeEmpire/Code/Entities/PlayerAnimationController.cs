using System;
using UnityEngine;

namespace AnimeEmpire.Entities
{
    /// Animator-parameter driven. Locomotion via `Speed` float (idle↔walk↔run
    /// transitions in Animator). Discrete actions via triggers (Sit, Gather,
    /// Stand, Celebrate, ToIdle). Carrying flag via Bool. work_sit auto-
    /// transitions to work_gather inside Animator (exit time on sit clip).
    public class PlayerAnimationController : MonoBehaviour
    {
        public const string StateIdle = "idle";
        public const string StateWalk = "walk";
        public const string StateRun = "run";
        public const string StateWorkSit = "work_sit";
        public const string StateWorkGather = "work_gather";
        public const string StateWorkStand = "work_stand";
        public const string StateCarryWalk = "carry_walk";
        public const string StateCelebrate = "celebrate";

        public const float SpeedThresholdWalk = 0.05f;
        public const float SpeedThresholdRun = 0.85f;

        // Animator parameter names — must match AnimatorControllerBuilder.
        public const string ParamSpeed = "Speed";
        public const string ParamCarrying = "Carrying";
        public const string TriggerSit = "Sit";
        public const string TriggerGather = "Gather";
        public const string TriggerStand = "Stand";
        public const string TriggerCelebrate = "Celebrate";
        public const string TriggerIdle = "ToIdle";

        [SerializeField] Animator _animator;
        public event Action<string> StateFinished;

        static readonly int HashSpeed = Animator.StringToHash(ParamSpeed);
        static readonly int HashCarrying = Animator.StringToHash(ParamCarrying);
        static readonly int HashSit = Animator.StringToHash(TriggerSit);
        static readonly int HashGather = Animator.StringToHash(TriggerGather);
        static readonly int HashStand = Animator.StringToHash(TriggerStand);
        static readonly int HashCelebrate = Animator.StringToHash(TriggerCelebrate);
        static readonly int HashIdle = Animator.StringToHash(TriggerIdle);

        void Awake()
        {
            if (_animator == null) _animator = GetComponentInChildren<Animator>();
        }

        public void UpdateSpeed(float speedNormalized)
        {
            if (_animator == null) return;
            _animator.SetFloat(HashSpeed, Mathf.Clamp01(speedNormalized));
        }

        public void SetCarrying(bool carrying)
        {
            if (_animator == null) return;
            _animator.SetBool(HashCarrying, carrying);
        }

        /// Discrete state trigger. Animator transitions w/ Any State → target.
        public void Trigger(string state)
        {
            if (_animator == null) return;
            int hash = state switch
            {
                StateWorkSit => HashSit,
                StateWorkGather => HashGather,
                StateWorkStand => HashStand,
                StateCelebrate => HashCelebrate,
                StateIdle => HashIdle,
                _ => 0,
            };
            if (hash != 0) _animator.SetTrigger(hash);
        }

        /// Backward-compat: Override = Trigger discrete state.
        public void Override(string state, float duration = -1f) => Trigger(state);

        public void ClearOverride()
        {
            if (_animator == null) return;
            _animator.SetTrigger(HashIdle);
        }

        // Animation Event hook (end of one-shot clips).
        public void OnAnimationStateFinished(string state)
            => StateFinished?.Invoke(state);
    }
}

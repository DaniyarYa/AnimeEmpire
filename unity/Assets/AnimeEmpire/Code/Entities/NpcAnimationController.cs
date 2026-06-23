using System;
using UnityEngine;

namespace AnimeEmpire.Entities
{
    /// Same Animator-parameter contract as PlayerAnimationController.
    /// NPC FSM drives via Speed float + work cycle triggers.
    public class NpcAnimationController : MonoBehaviour
    {
        [SerializeField] Animator _animator;
        public event Action<string> StateFinished;

        static readonly int HashSpeed = Animator.StringToHash(PlayerAnimationController.ParamSpeed);
        static readonly int HashCarrying = Animator.StringToHash(PlayerAnimationController.ParamCarrying);
        static readonly int HashSit = Animator.StringToHash(PlayerAnimationController.TriggerSit);
        static readonly int HashGather = Animator.StringToHash(PlayerAnimationController.TriggerGather);
        static readonly int HashStand = Animator.StringToHash(PlayerAnimationController.TriggerStand);
        static readonly int HashCelebrate = Animator.StringToHash(PlayerAnimationController.TriggerCelebrate);
        static readonly int HashIdle = Animator.StringToHash(PlayerAnimationController.TriggerIdle);

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

        public void Trigger(string state)
        {
            if (_animator == null) return;
            int hash = state switch
            {
                PlayerAnimationController.StateWorkSit => HashSit,
                PlayerAnimationController.StateWorkGather => HashGather,
                PlayerAnimationController.StateWorkStand => HashStand,
                PlayerAnimationController.StateCelebrate => HashCelebrate,
                PlayerAnimationController.StateIdle => HashIdle,
                _ => 0,
            };
            if (hash != 0) _animator.SetTrigger(hash);
        }

        public void Override(string state, float duration = -1f) => Trigger(state);

        public void ClearOverride()
        {
            if (_animator == null) return;
            _animator.SetTrigger(HashIdle);
        }

        public void OnAnimationStateFinished(string state)
            => StateFinished?.Invoke(state);
    }
}

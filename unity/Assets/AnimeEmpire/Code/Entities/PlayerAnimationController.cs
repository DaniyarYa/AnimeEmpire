using System;
using System.Collections;
using UnityEngine;

namespace AnimeEmpire.Entities
{
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
        public const float CrossFadeDuration = 0.1f;

        [SerializeField] Animator _animator;
        public event Action<string> StateFinished;

        string _currentState = "";
        string _overrideState = "";

        void Awake()
        {
            if (_animator == null) _animator = GetComponentInChildren<Animator>();
        }

        void Start() => PlayState(StateIdle);

        public void UpdateSpeed(float speedNormalized)
        {
            if (!string.IsNullOrEmpty(_overrideState)) return;
            string target = StateIdle;
            if (speedNormalized >= SpeedThresholdRun) target = StateRun;
            else if (speedNormalized >= SpeedThresholdWalk) target = StateWalk;
            PlayState(target);
        }

        public void Override(string state, float duration = -1f)
        {
            _overrideState = state;
            _currentState = "";
            PlayState(state);
            if (duration > 0f) StartCoroutine(ClearOverrideAfter(duration));
        }

        IEnumerator ClearOverrideAfter(float t)
        {
            yield return new WaitForSeconds(t);
            _overrideState = "";
        }

        public void ClearOverride()
        {
            _overrideState = "";
            _currentState = "";
        }

        void PlayState(string state)
        {
            if (state == _currentState) return;
            if (_animator == null) return;
            _currentState = state;
            int hash = Animator.StringToHash(state);
            if (!_animator.HasState(0, hash))
            {
                if (state != StateIdle) PlayState(StateIdle);
                return;
            }
            _animator.CrossFadeInFixedTime(hash, CrossFadeDuration);
        }

        // Hooked from AnimationEvent at end of one-shot clips (work_sit, work_stand, celebrate).
        // Signature: OnAnimationStateFinished(string stateName).
        public void OnAnimationStateFinished(string state)
        {
            StateFinished?.Invoke(string.IsNullOrEmpty(_overrideState) ? state : _overrideState);
        }
    }
}

using System;
using System.Collections;
using UnityEngine;

namespace AnimeEmpire.Entities
{
    public class NpcAnimationController : MonoBehaviour
    {
        public const float CrossFadeDuration = 0.1f;

        [SerializeField] Animator _animator;
        public event Action<string> StateFinished;

        string _currentState = "";
        string _overrideState = "";

        void Awake()
        {
            if (_animator == null) _animator = GetComponentInChildren<Animator>();
        }

        public void UpdateSpeed(float speedNormalized)
        {
            if (!string.IsNullOrEmpty(_overrideState)) return;
            string target = PlayerAnimationController.StateIdle;
            if (speedNormalized >= PlayerAnimationController.SpeedThresholdRun) target = PlayerAnimationController.StateRun;
            else if (speedNormalized >= PlayerAnimationController.SpeedThresholdWalk) target = PlayerAnimationController.StateWalk;
            Play(target);
        }

        public void Override(string state, float duration = -1f)
        {
            _overrideState = state;
            _currentState = "";
            Play(state);
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

        void Play(string state)
        {
            if (state == _currentState) return;
            if (_animator == null) return;
            _currentState = state;
            int hash = Animator.StringToHash(state);
            if (!_animator.HasState(0, hash)) return;
            _animator.CrossFadeInFixedTime(hash, CrossFadeDuration);
        }

        public void OnAnimationStateFinished(string state)
            => StateFinished?.Invoke(string.IsNullOrEmpty(_overrideState) ? state : _overrideState);
    }
}

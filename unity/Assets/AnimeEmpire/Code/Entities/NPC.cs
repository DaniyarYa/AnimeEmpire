using System.Collections.Generic;
using AnimeEmpire.Core;
using AnimeEmpire.Data;
using UnityEngine;

namespace AnimeEmpire.Entities
{
    public enum NpcState { Idle, Move, WorkSit, WorkGather, WorkStand, Carry, Deliver, Return }

    [RequireComponent(typeof(CharacterController))]
    public class NPC : MonoBehaviour
    {
        public const float ArrivalThreshold = 0.5f;
        public const float AccelLerp = 10f;
        public const float RotateLerp = 10f;
        public const float DeliverDuration = 0.4f;
        public const float SitDuration = 1f;
        public const float StandDuration = 1f;
        public static readonly Vector3 StorageArrivalOffset = new(0, 0, 1.5f);

        public NPCDef Def;
        public Building AssignedBuilding;
        public Vector3 WorkOffset = new(2.5f, 0, 0);

        [SerializeField] NpcAnimationController _anim;

        CharacterController _cc;
        NpcState _state = NpcState.Idle;
        Vector3 _velocity;
        Vector3 _targetPos;
        Vector3 _workPos;
        Building _storageTarget;
        float _workTimer;
        float _workDuration = 1f;
        float _speed = 2f;
        float _stateTimer;
        float _deliverPause;
        bool _standRequested;
        bool _dismissPending;
        readonly Dictionary<string, int> _carried = new();

        void Awake()
        {
            _cc = GetComponent<CharacterController>();
            if (_anim == null) _anim = GetComponentInChildren<NpcAnimationController>();
            NpcRegistry.Register(this);
        }

        void Start()
        {
            if (Def != null) _speed = Def.BaseSpeed;
            if (_anim != null) _anim.StateFinished += OnAnimStateFinished;
            if (AssignedBuilding != null)
            {
                BindToBuilding(AssignedBuilding);
                AssignedBuilding.SetWorker(this);
                EnterState(NpcState.Move);
            }
            else
            {
                EnterState(NpcState.Idle);
            }
        }

        void OnDestroy()
        {
            if (_anim != null) _anim.StateFinished -= OnAnimStateFinished;
            NpcRegistry.Unregister(this);
        }

        void BindToBuilding(Building b)
        {
            AssignedBuilding = b;
            _workPos = b.transform.position + WorkOffset;
            _targetPos = _workPos;
            _storageTarget = BuildingRegistry.FindFirstByCategory("service");
        }

        public void Assign(Building b)
        {
            if (b == null) return;
            _dismissPending = false;
            _standRequested = false;
            _carried.Clear();
            BindToBuilding(b);
            b.SetWorker(this);
            EnterState(NpcState.Move);
        }

        public bool IsAvailable() => _state == NpcState.Idle && !_dismissPending;

        public void Dismiss()
        {
            _dismissPending = true;
            switch (_state)
            {
                case NpcState.WorkSit:
                case NpcState.WorkGather:
                    _standRequested = true; break;
                case NpcState.Move:
                    EnterState(NpcState.Return); break;
            }
        }

        void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            switch (_state)
            {
                case NpcState.Idle: break;
                case NpcState.Move: if (WalkToward(dt)) EnterState(NpcState.WorkSit); break;
                case NpcState.WorkSit: ProcessSit(dt); break;
                case NpcState.WorkGather: ProcessGather(dt); break;
                case NpcState.WorkStand: ProcessStand(dt); break;
                case NpcState.Carry: if (WalkToward(dt)) EnterState(NpcState.Deliver); break;
                case NpcState.Deliver: ProcessDeliver(dt); break;
                case NpcState.Return: ProcessReturn(dt); break;
            }
        }

        void ProcessSit(float dt)
        {
            _stateTimer += dt;
            if (_stateTimer >= SitDuration)
                EnterState(_standRequested ? NpcState.WorkStand : NpcState.WorkGather);
        }

        void ProcessStand(float dt)
        {
            _stateTimer += dt;
            if (_stateTimer < StandDuration) return;
            if (CarriedTotal() > 0) EnterState(NpcState.Carry);
            else if (_dismissPending) EnterState(NpcState.Idle);
            else EnterState(NpcState.WorkSit);
        }

        void ProcessGather(float dt)
        {
            _workTimer += dt;
            if (_workTimer >= _workDuration)
            {
                _workTimer -= _workDuration;
                AccumulateCarried();
            }
            if (Def != null && CarriedTotal() >= Def.BaseCapacity) _standRequested = true;
            if (_standRequested) EnterState(NpcState.WorkStand);
        }

        void ProcessDeliver(float dt)
        {
            _deliverPause -= dt;
            if (_deliverPause <= 0f) EnterState(NpcState.Return);
        }

        void ProcessReturn(float dt)
        {
            if (AssignedBuilding == null) { EnterState(NpcState.Idle); return; }
            if (WalkToward(dt))
                EnterState(_dismissPending ? NpcState.Idle : NpcState.WorkSit);
        }

        void EnterState(NpcState s)
        {
            _state = s;
            switch (s)
            {
                case NpcState.Idle:
                    _velocity = Vector3.zero;
                    _standRequested = false;
                    _carried.Clear();
                    _dismissPending = false;
                    _anim?.ClearOverride();
                    _anim?.UpdateSpeed(0f);
                    break;
                case NpcState.WorkSit:
                    _velocity = Vector3.zero;
                    _standRequested = false;
                    _stateTimer = 0f;
                    _anim?.Override(PlayerAnimationController.StateWorkSit);
                    break;
                case NpcState.WorkGather:
                    _velocity = Vector3.zero;
                    _stateTimer = 0f;
                    if (AssignedBuilding != null && AssignedBuilding.Def != null)
                    {
                        float eff = Def != null ? Mathf.Max(0.01f, Def.BaseEfficiency) : 0.75f;
                        _workDuration = AssignedBuilding.Def.BaseCycleSeconds / eff;
                    }
                    _anim?.Override(PlayerAnimationController.StateWorkGather);
                    break;
                case NpcState.WorkStand:
                    _velocity = Vector3.zero;
                    _stateTimer = 0f;
                    _anim?.Override(PlayerAnimationController.StateWorkStand);
                    break;
                case NpcState.Carry:
                    _velocity = Vector3.zero;
                    _targetPos = _storageTarget != null
                        ? _storageTarget.transform.position + StorageArrivalOffset
                        : transform.position;
                    _anim?.Override(PlayerAnimationController.StateCarryWalk);
                    break;
                case NpcState.Deliver:
                    _velocity = Vector3.zero;
                    _deliverPause = DeliverDuration;
                    DepositCarried();
                    _anim?.ClearOverride();
                    _anim?.UpdateSpeed(0f);
                    break;
                case NpcState.Return:
                    _targetPos = _workPos;
                    _anim?.ClearOverride();
                    break;
            }
        }

        bool WalkToward(float dt)
        {
            Vector3 to = _targetPos - transform.position;
            to.y = 0f;
            if (to.magnitude < ArrivalThreshold) { _velocity = Vector3.zero; return true; }
            Vector3 dir = to.normalized;
            Vector3 target = dir * _speed;
            _velocity = Vector3.Lerp(_velocity, target, Mathf.Clamp01(AccelLerp * dt));
            _cc.Move((_velocity + Physics.gravity) * dt);
            var look = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, Mathf.Clamp01(RotateLerp * dt));
            _anim?.UpdateSpeed(_velocity.magnitude / Mathf.Max(_speed, 0.0001f));
            return false;
        }

        void AccumulateCarried()
        {
            if (AssignedBuilding == null || AssignedBuilding.Def == null) return;
            var b = AssignedBuilding.Def;
            if (b.OutputResource == null) return;
            string rid = b.OutputResource.Id;
            _carried.TryGetValue(rid, out var cur);
            _carried[rid] = cur + b.OutputAmount;
        }

        int CarriedTotal()
        {
            int sum = 0;
            foreach (var v in _carried.Values) sum += v;
            return sum;
        }

        void DepositCarried()
        {
            if (_carried.Count == 0) return;
            string bid = AssignedBuilding != null && AssignedBuilding.Def != null ? AssignedBuilding.Def.Id : "unknown";
            foreach (var kv in _carried) EventBus.RaiseResourceProduced(bid, kv.Key, kv.Value);
            _carried.Clear();
        }

        void OnAnimStateFinished(string state)
        {
            switch (state)
            {
                case PlayerAnimationController.StateWorkSit:
                    if (_state != NpcState.WorkSit) return;
                    EnterState(_standRequested ? NpcState.WorkStand : NpcState.WorkGather);
                    break;
                case PlayerAnimationController.StateWorkGather:
                    if (_state != NpcState.WorkGather) return;
                    if (_standRequested) EnterState(NpcState.WorkStand);
                    else _anim?.Override(PlayerAnimationController.StateWorkGather);
                    break;
                case PlayerAnimationController.StateWorkStand:
                    if (_state != NpcState.WorkStand) return;
                    if (CarriedTotal() > 0) EnterState(NpcState.Carry);
                    else if (_dismissPending) EnterState(NpcState.Idle);
                    else EnterState(NpcState.WorkSit);
                    break;
            }
        }
    }
}

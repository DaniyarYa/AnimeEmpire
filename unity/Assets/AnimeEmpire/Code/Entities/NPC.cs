using System.Collections.Generic;
using AnimeEmpire.Core;
using AnimeEmpire.Data;
using UnityEngine;
using UnityEngine.AI;

namespace AnimeEmpire.Entities
{
    public enum NpcState { Idle, Move, WorkSit, WorkGather, WorkStand, Carry, Deliver, Return }

    [RequireComponent(typeof(NavMeshAgent))]
    public class NPC : MonoBehaviour
    {
        public const float ArrivalThreshold = 0.5f;
        public const float DeliverDuration = 0.4f;
        public const float SitDuration = 1f;
        public const float StandDuration = 1f;
        public static readonly Vector3 StorageArrivalOffset = new(0, 0, 1.5f);

        public NPCDef Def;
        public Building AssignedBuilding;
        public Vector3 WorkOffset = new(2.5f, 0, 0);

        [SerializeField] NpcAnimationController _anim;

        NavMeshAgent _agent;
        NpcState _state = NpcState.Idle;
        Vector3 _targetPos;
        Vector3 _workPos;
        Building _storageTarget;
        float _workTimer;
        float _workDuration = 1f;
        float _stateTimer;
        float _deliverPause;
        bool _standRequested;
        bool _dismissPending;
        bool _destinationSet;
        readonly Dictionary<string, int> _carried = new();

        void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            if (_anim == null) _anim = GetComponentInChildren<NpcAnimationController>();
            NpcRegistry.Register(this);
        }

        void Start()
        {
            if (Def != null) _agent.speed = Def.BaseSpeed;
            _agent.angularSpeed = 540f;
            _agent.acceleration = 12f;
            _agent.stoppingDistance = ArrivalThreshold;
            _agent.autoBraking = true;

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

        void Update()
        {
            switch (_state)
            {
                case NpcState.Idle: break;
                case NpcState.Move: if (WalkToward()) EnterState(NpcState.WorkSit); break;
                case NpcState.WorkSit: ProcessSit(Time.deltaTime); break;
                case NpcState.WorkGather: ProcessGather(Time.deltaTime); break;
                case NpcState.WorkStand: ProcessStand(Time.deltaTime); break;
                case NpcState.Carry: if (WalkToward()) EnterState(NpcState.Deliver); break;
                case NpcState.Deliver: ProcessDeliver(Time.deltaTime); break;
                case NpcState.Return: ProcessReturn(); break;
            }

            // Drive Animator Speed parameter from agent velocity.
            if (_anim != null)
            {
                float maxSpeed = Mathf.Max(_agent.speed, 0.0001f);
                _anim.UpdateSpeed(_agent.velocity.magnitude / maxSpeed);
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

        void ProcessReturn()
        {
            if (AssignedBuilding == null) { EnterState(NpcState.Idle); return; }
            if (WalkToward())
                EnterState(_dismissPending ? NpcState.Idle : NpcState.WorkSit);
        }

        void EnterState(NpcState s)
        {
            _state = s;
            switch (s)
            {
                case NpcState.Idle:
                    StopAgent();
                    _standRequested = false;
                    _carried.Clear();
                    _dismissPending = false;
                    _anim?.SetCarrying(false);
                    _anim?.ClearOverride();
                    break;
                case NpcState.Move:
                    SetDestination(_workPos);
                    break;
                case NpcState.WorkSit:
                    StopAgent();
                    _standRequested = false;
                    _stateTimer = 0f;
                    _anim?.Trigger(PlayerAnimationController.StateWorkSit);
                    break;
                case NpcState.WorkGather:
                    StopAgent();
                    _stateTimer = 0f;
                    if (AssignedBuilding != null && AssignedBuilding.Def != null)
                    {
                        float eff = Def != null ? Mathf.Max(0.01f, Def.BaseEfficiency) : 0.75f;
                        _workDuration = AssignedBuilding.Def.BaseCycleSeconds / eff;
                    }
                    _anim?.Trigger(PlayerAnimationController.StateWorkGather);
                    break;
                case NpcState.WorkStand:
                    StopAgent();
                    _stateTimer = 0f;
                    _anim?.SetCarrying(CarriedTotal() > 0);
                    _anim?.Trigger(PlayerAnimationController.StateWorkStand);
                    break;
                case NpcState.Carry:
                    var carryTarget = _storageTarget != null
                        ? _storageTarget.transform.position + StorageArrivalOffset
                        : transform.position;
                    SetDestination(carryTarget);
                    _anim?.SetCarrying(true);
                    break;
                case NpcState.Deliver:
                    StopAgent();
                    _deliverPause = DeliverDuration;
                    DepositCarried();
                    _anim?.SetCarrying(false);
                    _anim?.ClearOverride();
                    break;
                case NpcState.Return:
                    SetDestination(_workPos);
                    _anim?.ClearOverride();
                    break;
            }
        }

        void SetDestination(Vector3 pos)
        {
            _targetPos = pos;
            if (_agent.isOnNavMesh)
            {
                _agent.isStopped = false;
                _agent.SetDestination(pos);
                _destinationSet = true;
            }
            else
            {
                _destinationSet = false;
            }
        }

        void StopAgent()
        {
            _destinationSet = false;
            if (_agent.isOnNavMesh)
            {
                _agent.ResetPath();
                _agent.isStopped = true;
            }
        }

        bool WalkToward()
        {
            if (!_agent.isOnNavMesh) return false;
            if (!_destinationSet)
            {
                _agent.SetDestination(_targetPos);
                _destinationSet = true;
                return false;
            }
            if (_agent.pathPending) return false;
            return _agent.remainingDistance <= _agent.stoppingDistance + 0.05f && _agent.velocity.sqrMagnitude < 0.01f;
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

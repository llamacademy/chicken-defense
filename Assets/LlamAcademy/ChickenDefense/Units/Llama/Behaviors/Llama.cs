using System.Collections.Generic;
using LlamAcademy.ChickenDefense.Units.FSM.Common;
using LlamAcademy.ChickenDefense.Units.FSM.Sensors;
using LlamAcademy.ChickenDefense.Units.Llama.FSM;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Pool;
using UnityHFSM;

namespace LlamAcademy.ChickenDefense.Units.Llama.Behaviors
{
    public class Llama : UnitBase<LlamaStates>
    {
        [SerializeField] private LlamaStates State;

        [Space] [SerializeField] private TargetSensor EnemySensor;
        [SerializeField] private TwoBoneIKConstraint LeftLegConstraint;
        [SerializeField] private TwoBoneIKConstraint RightLegConstraint;
        [SerializeField] private Transform SpitSpawnLocation;

        private List<IDamageable> NearbyEnemies = new();

        [field: SerializeField] public AnimationCurve StompHeightCurve { get; private set; }
        private HybridStateMachine<LlamaStates, StateEvent> AttackStateMachine;
        private ObjectPool<Spit> SpitPool;

        protected override void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
            AttackStateMachine = new HybridStateMachine<LlamaStates, StateEvent>();
            AddAttackStates();
            AddAttackTransitions();
            base.Awake();
        }

        private void Start()
        {
            EnemySensor.Collider.radius = Unit.AttackConfig.SensorRadius;
            EnemySensor.OnTargetEnter += OnTargetEnterRadius;
            EnemySensor.OnTargetExit += OnTargetExitRadius;
            if (Unit.AttackConfig.IsRanged)
            {
                SpitPool = new ObjectPool<Spit>(CreateSpitObject);
            }
        }

        protected override void Update()
        {
            base.Update();
            State = FSM.ActiveStateName;
        }

        private void AddAttackStates()
        {
            if (Unit.AttackConfig.IsRanged)
            {
                AttackStateMachine.AddState(LlamaStates.Attack,
                    new RangedAttackState(this));
            }
            else
            {
                AttackStateMachine.AddState(LlamaStates.Attack,
                    new MeleeAttackState(this, OnTargetDie, LeftLegConstraint, RightLegConstraint));
            }

            AttackStateMachine.AddState(LlamaStates.Move, new MoveState<LlamaStates>(this));
        }

        private void AddAttackTransitions()
        {
            AttackStateMachine.AddTransition(new Transition<LlamaStates>(LlamaStates.Idle, LlamaStates.Move,
                IsNotCloseEnoughToAttack));
            AttackStateMachine.AddTransition(new Transition<LlamaStates>(LlamaStates.Idle, LlamaStates.Attack,
                IsCloseEnoughToAttack));
            AttackStateMachine.AddTransition(new Transition<LlamaStates>(LlamaStates.Move, LlamaStates.Attack,
                IsCloseEnoughToAttack));
            AttackStateMachine.AddTransition(new Transition<LlamaStates>(LlamaStates.Attack, LlamaStates.Move,
                IsNotCloseEnoughToAttack));
        }

        protected override void AddStates()
        {
            FSM.AddState(LlamaStates.Idle, new IdleState<LlamaStates>(this));
            FSM.AddState(LlamaStates.Move, new MoveState<LlamaStates>(this));
            FSM.AddState(LlamaStates.Attack,
                AttackStateMachine); // ideally have exit time so animation for stomp completes

            FSM.SetStartState(LlamaStates.Idle);
        }

        protected override void AddTransitions()
        {
            FSM.AddTriggerTransitionFromAny(StateEvent.MoveIssued, LlamaStates.Move);
            FSM.AddTriggerTransitionFromAny(StateEvent.StopIssued, LlamaStates.Idle);
            FSM.AddTriggerTransitionFromAny(StateEvent.AttackIssued, LlamaStates.Attack);

            FSM.AddTransition(new Transition<LlamaStates>(LlamaStates.Move, LlamaStates.Attack, IsCloseEnoughToAttack));
            FSM.AddTransition(new Transition<LlamaStates>(LlamaStates.Idle, LlamaStates.Attack, IsCloseEnoughToAttack));
            // FSM.AddTransition(new Transition<LlamaStates>(LlamaStates.Attack, LlamaStates.Move,
            //     IsNotCloseEnoughToAttack));
            FSM.AddTransition(new Transition<LlamaStates>(LlamaStates.Move, LlamaStates.Idle, IsCloseToTarget));
        }

        private void OnTargetDie()
        {
            if (TransformTarget != null)
            {
                NearbyEnemies.Remove(TransformTarget.GetComponent<IDamageable>());
            }

            if (NearbyEnemies.Count != 0)
            {
                TransformTarget = NearbyEnemies[0].Transform;
            }
            else
            {
                TransformTarget = null;
                Target = transform.position;
                FSM.Trigger(StateEvent.MoveIssued);
            }
        }

        private void OnTargetEnterRadius(Transform target)
        {
            if (target.TryGetComponent(out IDamageable damageable) && Agent.CalculatePath(target.position, new NavMeshPath()))
            {
                NearbyEnemies.Add(damageable);
                
                // Pick the closest one
                NearbyEnemies.Sort((a, b) =>
                    Vector3.SqrMagnitude(a.Transform.position - transform.position)
                        .CompareTo(Vector3.SqrMagnitude(b.Transform.position - transform.position)));
            }

            TransformTarget = NearbyEnemies[0].Transform;
            FSM.Trigger(StateEvent.AttackIssued);
        }

        private void OnTargetExitRadius(Transform target)
        {
            OnTargetDie();
        }

        /// <summary>
        /// Animation Event
        /// </summary>
        /// <param name="_"></param>
        private void SpawnSpit(int _)
        {
            Spit instance = SpitPool.Get();

            instance.Spawn(this, OnTargetDie, SpitSpawnLocation);
        }

        private Spit CreateSpitObject()
        {
            GameObject instance = Instantiate(Unit.AttackConfig.RangedPrefab);
            instance.gameObject.SetActive(false); // prevent auto spawning particles
            return instance.GetComponent<Spit>();
        }

        private bool IsCloseToTarget(Transition<LlamaStates> _) =>
            Agent.enabled && Agent.remainingDistance <= Agent.stoppingDistance;

        private bool IsCloseEnoughToAttack(Transition<LlamaStates> _) =>
            TransformTarget != null &&
            Vector3.Distance(TransformTarget.GetComponent<Collider>().ClosestPoint(transform.position),
                transform.position) <= Unit.AttackConfig.AttackRadius;

        private bool IsNotCloseEnoughToAttack(Transition<LlamaStates> _) => !IsCloseEnoughToAttack(_);
    }
}
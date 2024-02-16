using LlamAcademy.ChickenDefense.Units.Enemies.FSM.Common;
using LlamAcademy.ChickenDefense.Units.FSM.Common;
using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

namespace LlamAcademy.ChickenDefense.Units.Enemies.Snake.Behaviors
{
    public class Snake : EnemyBase
    {
        [SerializeField] private SnakeSplineAnimator SplineAnimator;
        // TODO: remove
        [SerializeField] private Transform DebugStartTarget;

        protected override void Awake()
        {
            base.Awake();
            TransformTarget = DebugStartTarget; // TODO: remove to let AI brain set it
        }

        private void Start()
        {
            FSM.Trigger(StateEvent.MoveIssued);
        }
        
        protected override void AddStates()
        {
            FSM.AddState(EnemyStates.Idle,new IdleState<EnemyStates>(this));
            FSM.AddState(EnemyStates.Move, new EnemyMoveState(this));

            FSM.SetStartState(EnemyStates.Idle);
        }

        protected override void AddTransitions()
        {
            FSM.AddTriggerTransitionFromAny(StateEvent.MoveIssued, EnemyStates.Move, null,
                (_) => SplineAnimator.enabled = true);
            FSM.AddTriggerTransitionFromAny(StateEvent.StopIssued, EnemyStates.Idle, (_) => SplineAnimator.enabled = true);

            FSM.AddTransition(new Transition<EnemyStates>(EnemyStates.Move, EnemyStates.Idle, IsCloseToTarget));
        }

        protected override void HandleLlamaEnter(Transform target)
        {
            NearbyLlamas.Add(target.GetComponent<NavMeshAgent>());
            // TODO: is this the right way? I can't ever remember
            NearbyLlamas.Sort((a, b) =>
                Mathf.FloorToInt(
                    Vector3.SqrMagnitude(b.transform.position - transform.position) -
                    Vector3.SqrMagnitude(a.transform.position - transform.position)));
        }

        protected override void HandleLlamaExit(Transform target)
        {
            NearbyLlamas.Remove(target.GetComponent<NavMeshAgent>());
        }

        protected override void HandleFoodEnter(Transform target)
        {
            NearbyFood.Add(target.GetComponent<NavMeshAgent>());
        }

        protected override void HandleFoodExit(Transform target)
        {
            NearbyFood.Remove(target.GetComponent<NavMeshAgent>());
        }
        
        private bool IsCloseToTarget(Transition<EnemyStates> _) =>
            Agent.enabled && Agent.remainingDistance <= Agent.stoppingDistance;
    }
}
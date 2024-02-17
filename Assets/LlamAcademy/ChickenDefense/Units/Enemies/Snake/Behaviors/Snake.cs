using LlamAcademy.ChickenDefense.Units.Chicken.Behaviors;
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
        private bool IsWandering = true;
        private Egg TargetEgg;
        
        public void GoToEgg(Egg egg)
        {
            TransformTarget = egg.transform;
            FSM.Trigger(StateEvent.MoveIssued);
            IsWandering = false;
        }

        public void Wander()
        {
            IsWandering = true;
            TransformTarget = null;
            Target = PickNearbyWanderPosition();
            FSM.Trigger(StateEvent.MoveIssued);
        }
        
        protected override void AddStates()
        {
            FSM.AddState(EnemyStates.Idle, new IdleState<EnemyStates>(this));
            FSM.AddState(EnemyStates.Move, new EnemyMoveState(this));
            FSM.SetStartState(EnemyStates.Idle);
        }

        protected override void AddTransitions()
        {
            FSM.AddTriggerTransitionFromAny(StateEvent.MoveIssued, EnemyStates.Move, null,
                (_) => SplineAnimator.enabled = true);
            FSM.AddTriggerTransitionFromAny(StateEvent.StopIssued, EnemyStates.Idle,
                (_) => SplineAnimator.enabled = false);

            FSM.AddTransition(new Transition<EnemyStates>(EnemyStates.Move, EnemyStates.Move, ShouldPickNewWanderLocation, (_) => SplineAnimator.enabled = true));
            // died
            FSM.AddTransition(new Transition<EnemyStates>(EnemyStates.Move, EnemyStates.Idle, ShouldTransitionToIdle, (_) =>
            {
                gameObject.SetActive(false);
                if (TransformTarget != null && TransformTarget.TryGetComponent(out Egg _))
                {
                    TransformTarget.gameObject.SetActive(false);
                }
            }));
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
        }

        protected override void HandleFoodExit(Transform target)
        {
        }

        private bool IsCloseToTarget(Transition<EnemyStates> _) =>
            Agent.enabled && Agent.remainingDistance <= Agent.stoppingDistance;

        private bool ShouldTransitionToIdle(Transition<EnemyStates> _) => !IsWandering && IsCloseToTarget(_);
        private bool ShouldPickNewWanderLocation(Transition<EnemyStates> _) => IsWandering && IsCloseToTarget(_);
        
        private Vector3 PickNearbyWanderPosition()
        {
            Vector2 randomPosition = Random.insideUnitCircle * 3f; 
            return transform.position + new Vector3(
                randomPosition.x,
                0,
                randomPosition.y
            );
        }
    }
}
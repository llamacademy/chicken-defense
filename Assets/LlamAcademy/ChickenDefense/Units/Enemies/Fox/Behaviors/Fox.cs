using LlamAcademy.ChickenDefense.Units.Enemies.FSM.Common;
using LlamAcademy.ChickenDefense.Units.FSM.Common;
using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;
using ChickenUnit = LlamAcademy.ChickenDefense.Units.Chicken.Behaviors.Chicken;

namespace LlamAcademy.ChickenDefense.Units.Enemies.Fox.Behaviors
{
    // This is mostly copy/paste from Snake, so probably possible to group more behaviors into EnemyBase.
    // I intentionally did not do this so it is possible to have more diverse enemies more easily without having to rip
    // stuff out of EnemyBase
    public class Fox : EnemyBase
    {
        protected override void AddStates()
        {
            FSM.AddState(EnemyStates.Idle, new IdleState<EnemyStates>(this));
            FSM.AddState(EnemyStates.Move, new EnemyMoveState(this));
            FSM.SetStartState(EnemyStates.Idle);
        }

        protected override void AddTransitions()
        {
            FSM.AddTriggerTransitionFromAny(StateEvent.MoveIssued, EnemyStates.Move);
            FSM.AddTriggerTransitionFromAny(StateEvent.StopIssued, EnemyStates.Idle);

            FSM.AddTransition(new Transition<EnemyStates>(EnemyStates.Move, EnemyStates.Move, ShouldPickNewWanderLocation));

            FSM.AddTriggerTransitionFromAny(StateEvent.Die, EnemyStates.Idle, null, (_) =>
            {
                gameObject.SetActive(false);
                if (TransformTarget != null && TransformTarget.TryGetComponent(out ChickenUnit _))
                {
                    TransformTarget.gameObject.SetActive(false);
                }
            });
        }

        protected override void HandleLlamaEnter(Transform target)
        {
            NearbyLlamas.Add(target.GetComponent<NavMeshAgent>());
            NearbyLlamas.Sort((a, b) =>
                Vector3.SqrMagnitude(b.transform.position - transform.position)
                    .CompareTo(
                        Vector3.SqrMagnitude(a.transform.position - transform.position)
                    )
            );
        }

        protected override void HandleLlamaExit(Transform target)
        {
            NearbyLlamas.Remove(target.GetComponent<NavMeshAgent>());
        }

        protected override void HandleFoodEnter(Transform target)
        {
            if (target == TransformTarget)
            {
                Agent.isStopped = true;
                Animator.CrossFadeInFixedTime(AnimatorStates.IDLE, 0.1f);
                Invoke(nameof(Disable), 1f);
            }
        }

        private void Disable()
        {
            FSM.Trigger(StateEvent.Die);
        }

        protected override void HandleFoodExit(Transform target)
        {
        }

        private bool IsCloseToTarget(Transition<EnemyStates> _) =>
            Agent.enabled && Agent.remainingDistance <= Agent.stoppingDistance;

        private bool ShouldPickNewWanderLocation(Transition<EnemyStates> _) => IsWandering && IsCloseToTarget(_);
    }
}
using UnityEngine;

namespace LlamAcademy.ChickenDefense.Units.FSM.Common
{
    public class MoveState<TStateType> : UnitStateBase<TStateType>
    {
        private Vector3 LastDestination;

        public MoveState(UnitBase<TStateType> unit) : base(unit) {}

        public override void OnEnter()
        {
            base.OnEnter();
            NavMeshAgent.isStopped = false;
            Animator.CrossFadeInFixedTime(AnimatorStates.WALK, 0.2f);
            // Just take the point if we click on the floor or if there's no collider
            // Otherwise, don't walk inside that object!
            LastDestination = GetMoveTargetLocation();
            NavMeshAgent.SetDestination(LastDestination);
        }

        private Vector3 GetMoveTargetLocation()
        {
            Vector3 target;
            if (Unit.TransformTarget == null)
            {
                target = Unit.Target;
            }
            else if (Unit.TransformTarget.gameObject.layer != LayerMask.GetMask("Floor") && Unit.TransformTarget.TryGetComponent(out Collider collider))
            {
                target = collider.ClosestPoint(Unit.transform.position);
            }
            else
            {
                target = Unit.TransformTarget.position;
            }

            return target;
        }

        public override void OnLogic()
        {
            base.OnLogic();
            Vector3 target = GetMoveTargetLocation();

            if (target != LastDestination)
            {
                LastDestination = target;
                NavMeshAgent.SetDestination(target);
            }
        }
    }
}
using LlamAcademy.ChickenDefense.Units.FSM.Common;
using UnityEngine;

namespace LlamAcademy.ChickenDefense.Units.Enemies.FSM.Common
{
    public class EnemyMoveState : UnitStateBase<EnemyStates>
    {
        public EnemyMoveState(UnitBase<EnemyStates> unit) : base(unit)
        {
            Enemy = unit as EnemyBase;
        }

        private Vector3 LastDestination;
        private EnemyBase Enemy;

        public override void OnEnter()
        {
            base.OnEnter();
            NavMeshAgent.isStopped = false;
            Animator.CrossFade(AnimatorStates.WALK, 0.15f);
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
            else if (Unit.TransformTarget.gameObject.layer != LayerMask.GetMask("Floor") &&
                     Unit.TransformTarget.TryGetComponent(out Collider collider))
            {
                target = collider.ClosestPoint(NavMeshAgent.transform.position);
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

            if (Enemy.NearbyLlamas.Count == 0)
            {
                return;
            }
            
            Vector3 directionToClosestLlama = (Enemy.NearbyLlamas[0].transform.position - Enemy.transform.position);
             
            Enemy.Agent.velocity = Vector3.Lerp(
                Enemy.Agent.desiredVelocity,
                -directionToClosestLlama.normalized * Enemy.Agent.speed * Enemy.Unit.RunAwayConfig.SpeedMultiplier,
                Mathf.Clamp01((Enemy.Unit.RunAwayConfig.RunCompletelyAwayDistance.x - directionToClosestLlama.magnitude) / Enemy.Unit.RunAwayConfig.RunCompletelyAwayDistance.y)
            );
        }
    }
}
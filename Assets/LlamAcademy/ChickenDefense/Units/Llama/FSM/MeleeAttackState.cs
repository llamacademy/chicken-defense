using System;
using System.Collections;
using LlamAcademy.ChickenDefense.Units.Enemies;
using LlamAcademy.ChickenDefense.Units.FSM.Common;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace LlamAcademy.ChickenDefense.Units.Llama.FSM
{
    public class MeleeAttackState : UnitStateBase<LlamaStates>
    {
        private float LastAttackTime;
        private IDamageable Damageable;
        private readonly Action OnTargetDie;

        private Coroutine AttackCoroutine;
        private readonly TwoBoneIKConstraint LeftLegConstraint;
        private readonly TwoBoneIKConstraint RightLegConstraint;

        public MeleeAttackState(Behaviors.Llama unit, Action onTargetDie, TwoBoneIKConstraint leftLegConstraint, TwoBoneIKConstraint rightLegConstraint) : base(unit)
        {
            OnTargetDie = onTargetDie;
            LeftLegConstraint = leftLegConstraint;
            RightLegConstraint = rightLegConstraint;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            Damageable = Unit.TransformTarget.GetComponent<IDamageable>();
            NavMeshAgent.isStopped = true;
            Animator.CrossFade(AnimatorStates.IDLE_ANIMATION, 0.10f);
        }

        public override void OnLogic()
        {
            base.OnLogic();

            if (Time.time > LastAttackTime + Unit.Unit.AttackConfig.AttackSpeed)
            {
                LastAttackTime = Time.time;
                
                if (AttackCoroutine != null)
                {
                    Unit.StopCoroutine(AttackCoroutine);
                    LeftLegConstraint.data.targetPositionWeight = 0;
                    RightLegConstraint.data.targetPositionWeight = 0;
                }

                float dot = Vector3.Dot(Unit.transform.forward,
                    (Unit.TransformTarget.position - Unit.transform.position).normalized);
                
                Unit.TransformTarget.GetComponent<EnemyBase>().GetAttacked();
                
                if (dot < 0)
                {
                    AttackCoroutine = Unit.StartCoroutine(PlayStompAnimation(RightLegConstraint));    
                }
                else
                {
                    AttackCoroutine = Unit.StartCoroutine(PlayStompAnimation(LeftLegConstraint));
                }
            }
        }

        private IEnumerator PlayStompAnimation(TwoBoneIKConstraint constraint)
        {
            Behaviors.Llama llama = Unit as Behaviors.Llama;
            Vector3 targetPosition = constraint.data.target.localPosition;
            float normalAnimationSpeed = 1.4f; // 1 second for primary stomp, .4 seconds to return to normal
            float speed = Mathf.Max(normalAnimationSpeed / Unit.Unit.AttackConfig.AttackSpeed, 1); // do not go below 1 speed
            
            constraint.data.target.localPosition = targetPosition;
            float time = 0;
            while (time < 1)
            {
                constraint.data.targetPositionWeight = 1;
                constraint.data.target.localPosition = new(
                    targetPosition.x,
                    targetPosition.y + llama.StompHeightCurve.Evaluate(time),
                    targetPosition.z
                );

                time += Time.deltaTime * speed;

                if (time > 0.5f)
                {
                    Damageable.TakeDamage(Unit.Unit.AttackConfig.Damage, Unit);
                
                }
                yield return null;
            }

            yield return null;
            constraint.data.targetPositionWeight = 0;
            while (time > 0)
            {
                time -= Time.deltaTime * 2.5f * speed;
                constraint.data.targetPositionWeight = Mathf.Lerp(0, 1, time);
                yield return null;
            }

            constraint.data.targetPositionWeight = 0;
            if (Damageable.CurrentHealth <= 0)
            {
                OnTargetDie();
            }
        }
    }
}
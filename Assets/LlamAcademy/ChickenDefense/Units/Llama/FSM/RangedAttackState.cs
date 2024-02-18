using LlamAcademy.ChickenDefense.Units.FSM.Common;
using UnityEngine;

namespace LlamAcademy.ChickenDefense.Units.Llama.FSM
{
    public class RangedAttackState : UnitStateBase<LlamaStates>
    {
        private float LastAttackTime;

        private Coroutine AttackCoroutine;

        public RangedAttackState(UnitBase<LlamaStates> unit) : base(unit) {}

        public override void OnEnter()
        {
            base.OnEnter();
            NavMeshAgent.isStopped = true;
            Animator.CrossFade(AnimatorStates.IDLE, 0.10f);
        }

        public override void OnLogic()
        {
            base.OnLogic();

            Quaternion lookRotation = Quaternion.LookRotation((Unit.TransformTarget.position - Unit.transform.position).normalized);
            Unit.transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
            
            if (Time.time > LastAttackTime + Unit.Unit.AttackConfig.AttackSpeed)
            {
                LastAttackTime = Time.time;
                
                Animator.CrossFade(AnimatorStates.SPIT_ATTACK, 0.1f);
            }
        }
    }
}
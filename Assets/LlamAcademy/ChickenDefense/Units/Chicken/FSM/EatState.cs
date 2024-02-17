using LlamAcademy.ChickenDefense.Units.FSM.Common;
using UnityEngine;

namespace LlamAcademy.ChickenDefense.Units.Chicken.FSM
{
    public class EatState : UnitStateBase<ChickenStates>
    {
        private Behaviors.Chicken Chicken;

        public EatState(UnitBase<ChickenStates> unit) : base(unit)
        {
            Chicken = Unit as Behaviors.Chicken;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Animator.CrossFadeInFixedTime(AnimatorStates.EAT, 0.1f);
            NavMeshAgent.isStopped = true;
        }

        public override void OnLogic()
        {
            base.OnLogic();
            Chicken.Hunger -= Time.deltaTime * Chicken.HungerRegenerationRate;
        }
    }
}
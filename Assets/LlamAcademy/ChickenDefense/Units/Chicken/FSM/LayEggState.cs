using LlamAcademy.ChickenDefense.Units.FSM.Common;

namespace LlamAcademy.ChickenDefense.Units.Chicken.FSM
{
    public class LayEggState : UnitStateBase<ChickenStates>
    {
        public LayEggState(UnitBase<ChickenStates> unit) : base(unit)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Animator.CrossFadeInFixedTime(AnimatorStates.LAY_EGG, 0.1f);
            NavMeshAgent.isStopped = true;
        }
    }
}
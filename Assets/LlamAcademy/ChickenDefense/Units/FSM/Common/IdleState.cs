namespace LlamAcademy.ChickenDefense.Units.FSM.Common
{
    public class IdleState<TStateType> : UnitStateBase<TStateType>
    {
        public IdleState(UnitBase<TStateType> unit) : base(unit)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (NavMeshAgent.enabled && NavMeshAgent.isOnNavMesh)
            {
                NavMeshAgent.isStopped = true;
            }

            Animator.CrossFadeInFixedTime(AnimatorStates.IDLE, 0.2f);
        }
    }
}
using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

namespace LlamAcademy.ChickenDefense.Units.FSM.Common
{
    public class UnitStateBase<TStateType> : State<TStateType, StateEvent>
    {
        protected UnitBase<TStateType> Unit;
        protected NavMeshAgent NavMeshAgent;
        protected Animator Animator;
        
        public UnitStateBase(UnitBase<TStateType> unit)
        {
            Unit = unit;
            NavMeshAgent = Unit.Agent;
            Animator = Unit.Animator;
        }
    }
}
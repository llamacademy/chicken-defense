using LlamAcademy.ChickenDefense.Units.FSM.Common;
using UnityEngine;
using UnityHFSM;

namespace LlamAcademy.ChickenDefense.Units
{
    public abstract class UnitBase<TStateType> : AbstractUnit
    {
        protected StateMachine<TStateType, StateEvent> FSM;
        protected static readonly int SPEED_PARAMETER = Animator.StringToHash("Speed");
        protected float SmoothSpeed = 0;

        protected override void Awake()
        {
            base.Awake();
            
            FSM = new();
            AddStates();
            AddTransitions();
            FSM.Init();
        }

        protected virtual void Update()
        {
            FSM.OnLogic();
            SmoothSpeed = Mathf.Lerp(SmoothSpeed, (Agent.enabled && Agent.hasPath) ? Agent.velocity.magnitude : 0,
                Time.deltaTime);
            Animator.SetFloat(SPEED_PARAMETER, SmoothSpeed);
        }

        protected abstract void AddStates();

        protected abstract void AddTransitions();
        
        public override void MoveTo(Vector3 target)
        {
            base.MoveTo(target);
            FSM.Trigger(StateEvent.MoveIssued);
        }

        public override void Follow(Transform target)
        {
            base.Follow(target);
            FSM.Trigger(StateEvent.MoveIssued);
        }

        public override void Attack(IDamageable damageable)
        {
            base.Attack(damageable);
            FSM.Trigger(StateEvent.AttackIssued);
        }

        public virtual void Stop()
        {
            FSM.Trigger(StateEvent.StopIssued);
        }
    }
}
using System.Collections.Generic;
using LlamAcademy.ChickenDefense.Units.FSM.Common;
using LlamAcademy.ChickenDefense.Units.FSM.Sensors;
using UnityEngine;
using UnityEngine.AI;

namespace LlamAcademy.ChickenDefense.Units.Enemies
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public abstract class EnemyBase : UnitBase<EnemyStates>
    {
        [SerializeField] private TargetSensor LlamaSensor;
        [SerializeField] private TargetSensor FoodSensor;
        public List<NavMeshAgent> NearbyLlamas = new();
        public List<NavMeshAgent> NearbyFood = new();
        public Vector2 RunCompletelyAwayDistance = new(5,2); // TODO: move to SO
        
        protected override void OnEnable()
        {
            base.OnEnable();
            LlamaSensor.OnTargetEnter += HandleLlamaEnter;
            LlamaSensor.OnTargetExit += HandleLlamaExit;
            FoodSensor.OnTargetEnter += HandleFoodEnter;
            FoodSensor.OnTargetEnter += HandleFoodEnter;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            LlamaSensor.OnTargetEnter -= HandleLlamaEnter;
            LlamaSensor.OnTargetExit -= HandleLlamaExit;
            FoodSensor.OnTargetEnter -= HandleFoodEnter;
            FoodSensor.OnTargetEnter -= HandleFoodExit;
        }

        public void GetAttacked()
        {
            FSM.Trigger(StateEvent.StopIssued);
        }
        
        public override void TakeDamage(float damage, AbstractUnit attacker)
        {
            base.TakeDamage(damage, attacker);

            if (CurrentHealth > 0)
            {
                FSM.Trigger(StateEvent.MoveIssued);
            }
        }

        protected abstract void HandleLlamaEnter(Transform target);
        protected abstract void HandleLlamaExit(Transform target);
        protected abstract void HandleFoodEnter(Transform target);
        protected abstract void HandleFoodExit(Transform target);
    }
}
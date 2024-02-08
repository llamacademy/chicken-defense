using System;
using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Events;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

namespace LlamAcademy.ChickenDefense.Units
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public abstract class AbstractUnit : MonoBehaviour, IDamageable, IMoveable, ISelectable
    {
        public UnitSO Unit;
        
        [SerializeField] protected DecalProjector SelectionDecal;

        public Transform Transform => transform;

        [field: SerializeField] public float CurrentHealth { get; protected set; }
        [field: SerializeField] public float MaxHealth { get; protected set; }
        
        [field: Header("Debug Targets")]
        [field: SerializeField] public Transform TransformTarget { get; set; }
        [field: SerializeField] public Vector3 Target { get; set; }
        
        public NavMeshAgent Agent { get; protected set; }
        public Animator Animator { get; protected set; }

        protected virtual void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
        }

        public virtual void TakeDamage(float damage, AbstractUnit attackingUnit)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, CurrentHealth);
            
            if (CurrentHealth == 0)
            {
                Die();
            }
        }
        
        protected virtual void OnEnable()
        {
            SetupFromSO();
            Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
        }

        private void SetupFromSO()
        {
            MaxHealth = Unit.Health;
            CurrentHealth = Unit.Health;
        }

        protected virtual void OnDisable()
        {
            Bus<UnitDeathEvent>.Raise(new UnitDeathEvent(this));
        }

        protected virtual void Die()
        {
            gameObject.SetActive(false);
        }

        public virtual void MoveTo(Vector3 target)
        {
            TransformTarget = null;
            Target = target;
        }

        public virtual void Follow(Transform target)
        {
            TransformTarget = target;
            Target = Vector3.zero;
        }

        public virtual void Select()
        {
            SelectionDecal.fadeFactor = 1;
        }

        public virtual void Deselect()
        {
            SelectionDecal.fadeFactor = 0;
        }

        public virtual void OnMouseIn()
        {
            SelectionDecal.fadeFactor = 0.5f;
        }

        public virtual void OnMouseOut()
        {
            SelectionDecal.fadeFactor = 0;
        }
    }
}

using UnityEngine;

namespace LlamAcademy.ChickenDefense.Units
{
    public interface IDamageable
    {
        public Transform Transform { get; }
        public float CurrentHealth { get; }
        public float MaxHealth { get; }
        public void TakeDamage(float damage, AbstractUnit attackingUnit);
    }
}
using System;
using UnityEngine;

namespace LlamAcademy.ChickenDefense.Units.Llama.Behaviors
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class Spit : MonoBehaviour
    {
        [SerializeField] private float MoveSpeed = 5f;
        [SerializeField] private float AutoDisableTime = 5f;
        private Llama Llama;
        private Action OnTargetDie;
        private Rigidbody Rigidbody;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            CancelInvoke();
            Invoke(nameof(Disable), AutoDisableTime);
        }

        public void Spawn(Llama parent, Action onTargetDie, Transform spawnLocation)
        {
            Llama = parent;
            OnTargetDie += onTargetDie;
            transform.position = spawnLocation.position;
            transform.forward = spawnLocation.forward;
            Rigidbody.velocity = spawnLocation.forward * MoveSpeed;
            gameObject.SetActive(true);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(Llama.Unit.AttackConfig.Damage, Llama);
                if (damageable.CurrentHealth <= 0)
                {
                    OnTargetDie?.Invoke();    
                }
                
                gameObject.SetActive(false);
            }
        }

        private void Disable()
        {
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            OnTargetDie = null;
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
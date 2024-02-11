using UnityEngine;

namespace LlamAcademy.ChickenDefense.Units.FSM.Sensors
{
    [RequireComponent(typeof(SphereCollider))]
    public class TargetSensor : MonoBehaviour
    {
        public SphereCollider Collider;

        private void Awake()
        {
            Collider = GetComponent<SphereCollider>();
        }

        public delegate void TargetEnterEvent(Transform target);

        public delegate void TargetExitEvent(Transform target);

        public event TargetEnterEvent OnTargetEnter;
        public event TargetExitEvent OnTargetExit;

        private void OnTriggerEnter(Collider other)
        {
            OnTargetEnter?.Invoke(other.transform);
        }

        private void OnTriggerExit(Collider other)
        {
            OnTargetExit?.Invoke(other.transform);
        }
    }
}
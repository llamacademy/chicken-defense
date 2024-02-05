using UnityEngine;

namespace LlamAcademy.ChickenDefense.Units.FSM.Sensors
{
    public class TargetSensor : MonoBehaviour
    {
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
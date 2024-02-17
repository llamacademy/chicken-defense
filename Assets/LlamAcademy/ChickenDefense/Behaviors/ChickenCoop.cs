using UnityEngine;

namespace LlamAcademy.ChickenDefense.Behaviors
{
    [RequireComponent(typeof(Collider))]
    public class ChickenCoop : MonoBehaviour
    {
        private static Bounds _CoopBounds;
        
        public static Bounds CoopBounds => _CoopBounds;

        private void Awake()
        {
            _CoopBounds = GetComponent<Collider>().bounds;
        }
    }
}
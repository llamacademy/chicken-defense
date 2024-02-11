using UnityEngine;

namespace LlamAcademy.ChickenDefense.Units
{
    [CreateAssetMenu(menuName="Units/Attack Config", order = 0, fileName = "Attack Config")]
    public class AttackConfigSO : ScriptableObject
    {
        public float AttackSpeed = 2f;
        public float AttackRadius = 3f;
        public float SensorRadius = 5f;
        public float Damage = 10;
        
        [Space]
        public bool IsRanged = false;
        public GameObject RangedPrefab;

    }
}
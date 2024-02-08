using UnityEngine;

namespace LlamAcademy.ChickenDefense.Units
{
    [CreateAssetMenu(menuName="Units/Unit", order = 0, fileName = "Unit")]
    public class UnitSO : ScriptableObject
    {
        public float Health = 100;
        public float ResourceCost = 5;
        public NavMeshAgentConfigSO NavMeshAgentConfig;
        public AttackConfigSO AttackConfig;
    }
}
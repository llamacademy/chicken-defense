using UnityEngine;

namespace LlamAcademy.ChickenDefense.Units
{
    [CreateAssetMenu(menuName="Units/Unit", order = 0, fileName = "Unit")]
    public class UnitSO : ScriptableObject
    {
        public float Health = 100;
        public Texture2D UIIcon;
        public GameObject Prefab;
        public GameObject PlacementGhostPrefab;
        public ResourceCostSO ResourceCost;
        public NavMeshAgentConfigSO NavMeshAgentConfig;
        public AttackConfigSO AttackConfig;
        /// <summary>
        /// Defines how the AI unit should run away when high priority enemies come nearby.
        /// </summary>
        public RunAwayConfigSO RunAwayConfig;
    }
}
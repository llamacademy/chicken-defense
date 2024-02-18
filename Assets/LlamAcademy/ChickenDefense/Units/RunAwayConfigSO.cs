using UnityEngine;

namespace LlamAcademy.ChickenDefense.Units
{
    [CreateAssetMenu(menuName = "Units/Run Away Config", fileName = "Run Away Config", order = 6)]
    public class RunAwayConfigSO : ScriptableObject
    {
        public Vector2 RunCompletelyAwayDistance = new(5,2); 
        public float SpeedMultiplier = 1f;
    }
}
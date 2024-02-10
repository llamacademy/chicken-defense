using UnityEngine;

namespace LlamAcademy.ChickenDefense.Units
{
    [CreateAssetMenu(menuName = "Units/Resource Cost", order = 3, fileName = "Resource Cost")]
    public class ResourceCostSO : ScriptableObject
    {
        public int Cost = 5;
        public Texture2D Icon;
        public string Name = "Eggs";
    }
}
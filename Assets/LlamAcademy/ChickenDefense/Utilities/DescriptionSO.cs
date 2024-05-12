using UnityEngine;

namespace LlamAcademy.ChickenDefense.Utilities
{
    public abstract class DescriptionSO : ScriptableObject
    {
        [TextArea(5, 20)] [SerializeField] protected string Description;
    }
}

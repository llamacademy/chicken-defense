using UnityEngine;

namespace LlamAcademy.ChickenDefense.Units
{
    public interface IMoveable
    {
        void MoveTo(Vector3 target);
        void Follow(Transform target);
    }
}
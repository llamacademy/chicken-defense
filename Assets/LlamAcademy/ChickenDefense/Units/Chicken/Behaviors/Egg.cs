using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Events;
using UnityEngine;

namespace LlamAcademy.ChickenDefense.Units.Chicken.Behaviors
{
    public class Egg : MonoBehaviour
    {
        private void OnEnable()
        {
            Bus<EggSpawnEvent>.Raise(new EggSpawnEvent(this));
        }

        private void OnDisable()
        {
            Bus<EggRemovedEvent>.Raise(new EggRemovedEvent(this));
        }
    }
}
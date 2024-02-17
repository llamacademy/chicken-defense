using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Units.Chicken.Behaviors;

namespace LlamAcademy.ChickenDefense.Events
{
    public struct EggSpawnEvent : IEvent
    {
        public Egg Egg;

        public EggSpawnEvent(Egg egg)
        {
            Egg = egg;
        }
    }
}
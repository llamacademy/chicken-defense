using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Units.Chicken.Behaviors;

namespace LlamAcademy.ChickenDefense.Events
{
    public struct EggRemovedEvent : IEvent
    {
        public Egg[] Eggs;

        public EggRemovedEvent(Egg[] eggs)
        {
            Eggs = eggs;
        }

        public EggRemovedEvent(Egg egg)
        {
            Eggs = new[] { egg };
        }
    }
}
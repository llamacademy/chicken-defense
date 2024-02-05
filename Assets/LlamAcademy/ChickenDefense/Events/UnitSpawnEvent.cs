using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Units;

namespace LlamAcademy.ChickenDefense.Events
{
    public struct UnitSpawnEvent : IEvent
    {
        public AbstractUnit Unit;

        public UnitSpawnEvent(AbstractUnit unit)
        {
            Unit = unit;
        }
    }
}
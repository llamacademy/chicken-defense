using LlamAcademy.ChickenDefense.Units;
using LlamAcademy.ChickenDefense.EventBus;

namespace LlamAcademy.ChickenDefense.Events
{
    public struct UnitDeathEvent : IEvent
    {
        public AbstractUnit Unit;

        public UnitDeathEvent(AbstractUnit unit)
        {
            Unit = unit;
        }
    }
}
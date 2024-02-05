using LlamAcademy.ChickenDefense.Units;
using LlamAcademy.ChickenDefense.EventBus;

namespace LlamAcademy.ChickenDefense.Events
{
    public struct UnitDeselectedEvent : IEvent
    {
        public AbstractUnit[] Units;

        public UnitDeselectedEvent(AbstractUnit unit)
        {
            Units = new[] { unit };
        }

        public UnitDeselectedEvent(AbstractUnit[] units)
        {
            Units = units;
        }
    }
}
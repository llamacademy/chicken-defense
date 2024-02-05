using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Units;

namespace LlamAcademy.ChickenDefense.Events
{
    public struct UnitSelectedEvent : IEvent
    {
        public AbstractUnit[] Units;

        public UnitSelectedEvent(AbstractUnit unit)
        {
            Units = new[] { unit };
        }
        
        public UnitSelectedEvent(AbstractUnit[] units)
        {
            Units = units;
        }
    }
}
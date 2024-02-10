using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Units;

namespace LlamAcademy.ChickenDefense.Events
{
    public struct UnitSelectedToPlaceEvent : IEvent
    {
        public UnitSO Unit;

        public UnitSelectedToPlaceEvent(UnitSO unit)
        {
            Unit = unit;
        }
    }
}
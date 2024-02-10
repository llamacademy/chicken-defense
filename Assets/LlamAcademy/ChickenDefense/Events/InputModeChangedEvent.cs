using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Player;

namespace LlamAcademy.ChickenDefense.Events
{
    public struct InputModeChangedEvent : IEvent
    {
        public ActiveInputTarget NewTarget;

        public InputModeChangedEvent(ActiveInputTarget target)
        {
            NewTarget = target;
        }
    }
}
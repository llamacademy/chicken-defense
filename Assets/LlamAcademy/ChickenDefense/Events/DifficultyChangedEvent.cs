using LlamAcademy.ChickenDefense.AI;
using LlamAcademy.ChickenDefense.EventBus;

namespace LlamAcademy.ChickenDefense.Events
{
    public struct DifficultyChangedEvent : IEvent
    {
        public Difficulty Difficulty;

        public DifficultyChangedEvent(Difficulty difficulty)
        {
            Difficulty = difficulty;
        }
    }
}
using JetBrains.Annotations;

namespace LlamAcademy.ChickenDefense.EventBus
{
    public static class Bus<T> where T : IEvent
    {
        public delegate void Event(T evt);

        public static event Event OnEvent;

        public static void Raise([CanBeNull] T evt) => OnEvent?.Invoke(evt);
    }
}

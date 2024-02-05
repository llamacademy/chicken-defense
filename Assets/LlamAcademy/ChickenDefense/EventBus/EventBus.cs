using System.Collections.Generic;

namespace LlamAcademy.ChickenDefense.EventBus
{
    public static class Bus<T> where T : IEvent
    {
        private static readonly HashSet<IEventBinding<T>> Bindings = new();

        public static void Register(EventBinding<T> binding) => Bindings.Add(binding);
        public static void Unregister(EventBinding<T> binding) => Bindings.Remove(binding);

        public static void Raise(T @event)
        {
            foreach (IEventBinding<T> binding in Bindings)
            {
                binding.OnEventNoArgs?.Invoke();
                binding.OnEvent?.Invoke(@event);
            }
        }
    }
}
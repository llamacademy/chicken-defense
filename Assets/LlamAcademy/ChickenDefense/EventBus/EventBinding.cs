using System;

namespace LlamAcademy.ChickenDefense.EventBus
{
    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        public Action<T> OnEvent { get; set; } = (_) => { };
        public Action OnEventNoArgs { get; set; } = () => { };

        public EventBinding(Action<T> onEvent) => OnEvent = onEvent;
        public EventBinding(Action onEvent) => OnEventNoArgs = onEvent;

        public void Add(Action<T> onEvent) => OnEvent += onEvent;
        public void Add(Action onEvent) => OnEventNoArgs += onEvent;

        public void Remove(Action<T> onEvent) => OnEvent -= onEvent;
        public void Remove(Action onEvent) => OnEventNoArgs -= onEvent;
    }
}
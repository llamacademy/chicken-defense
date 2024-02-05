using System;

namespace LlamAcademy.ChickenDefense.EventBus
{
    public interface IEventBinding<T>
    {
        public Action<T> OnEvent { get; set; }
        public Action OnEventNoArgs { get; set; }
    }
}
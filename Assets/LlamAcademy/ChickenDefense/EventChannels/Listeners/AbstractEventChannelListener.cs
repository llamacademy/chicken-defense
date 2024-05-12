using UnityEngine;
using UnityEngine.Events;

namespace LlamAcademy.ChickenDefense.EventChannels.Listeners
{
    public abstract class AbstractEventChannelListener<TEventChannel, TEventType>
        : MonoBehaviour where TEventChannel : GenericEventChannelSO<TEventType>
    {
        [Header("Listen to Event Channels")]
        [SerializeField] protected TEventChannel EventChannel;
        [Tooltip("Responds to receiving signal from Event Channel")]
        [SerializeField] protected UnityEvent<TEventType> Response;
        protected virtual void OnEnable()
        {
            if (EventChannel != null)
            {
                EventChannel.OnEventRaised += OnEventRaised;
            }
        }

        protected virtual void OnDisable()
        {
            if (EventChannel != null)
            {
                EventChannel.OnEventRaised -= OnEventRaised;
            }
        }

        public void OnEventRaised(TEventType evt)
        {
            Response?.Invoke(evt);
        }
    }
}

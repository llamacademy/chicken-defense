using LlamAcademy.ChickenDefense.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace LlamAcademy.ChickenDefense.EventChannels
{
    /// <summary>
    /// General Event Channel that carries no extra data.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Void Event Channel", fileName = "Void Event Channel")]
    public class VoidEventChannelSO : DescriptionSO
    {
        [Tooltip("The action to perform")]
        public UnityAction OnEventRaised;

        public void RaiseEvent()
        {
            OnEventRaised?.Invoke();
        }
    }

}

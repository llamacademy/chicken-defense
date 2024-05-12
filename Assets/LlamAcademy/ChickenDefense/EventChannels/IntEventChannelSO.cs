using UnityEngine;

namespace LlamAcademy.ChickenDefense.EventChannels
{
    /// <summary>
    /// This is a ScriptableObject-based event that takes an integer as a payload.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Int EventChannel", fileName = "Int Event Channel")]
    public class IntEventChannelSO : GenericEventChannelSO<int>
    {
    }
}

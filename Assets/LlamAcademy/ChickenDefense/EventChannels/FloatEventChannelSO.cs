using UnityEngine;

namespace LlamAcademy.ChickenDefense.EventChannels
{
    /// <summary>
    /// A Scriptable Object-based event that passes a float as a payload.
    /// </summary>
    [CreateAssetMenu(fileName = "FloatEventChannel", menuName = "Events/Float Event Channel")]
    public class FloatEventChannelSO : GenericEventChannelSO<float>
    {
    }
}

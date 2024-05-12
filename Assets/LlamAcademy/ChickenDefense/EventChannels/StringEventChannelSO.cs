using UnityEngine;

namespace LlamAcademy.ChickenDefense.EventChannels
{
    /// <summary>
    /// General event channel that broadcasts and carries string payload.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/String Event Channel", fileName = "String Event Channel")]
    public class StringEventChannelSO : GenericEventChannelSO<string>
    {
    }
}

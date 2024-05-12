using UnityEngine;

namespace LlamAcademy.ChickenDefense.EventChannels
{
    /// <summary>
    /// This event channel broadcasts and carries Boolean payload.
    /// </summary>
    [CreateAssetMenu(fileName = "BoolEventChannelSO", menuName = "Events/Bool Event Channel")]
    public class BoolEventChannelSO : GenericEventChannelSO<bool>
    {
    }
}

using UnityEngine;

namespace LlamAcademy.ChickenDefense.EventChannels
{
    /// <summary>
    /// General event channel that broadcasts and carries Vector3 payload.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Vector3 Event Channel", fileName = "Vector3 Event Channel")]
    public class Vector3EventChannelSO : GenericEventChannelSO<Vector3>
    {
    }
}

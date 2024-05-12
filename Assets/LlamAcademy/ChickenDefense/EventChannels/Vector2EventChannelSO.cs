using UnityEngine;

namespace LlamAcademy.ChickenDefense.EventChannels
{
    /// <summary>
    /// General Event Channel that broadcasts and carries Vector2 payload.
    /// </summary>
    ///
    [CreateAssetMenu(menuName = "Events/Vector2 Event Channel", fileName = "Vector2 Event Channel")]
    public class Vector2EventChannelSO : GenericEventChannelSO<Vector2>
    {

    }
}

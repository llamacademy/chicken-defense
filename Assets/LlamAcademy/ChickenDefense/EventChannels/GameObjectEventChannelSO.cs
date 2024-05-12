using UnityEngine;

namespace LlamAcademy.ChickenDefense.EventChannels
{
    /// <summary>
    /// This is a ScriptableObject-based event that carries a GameObject as a payload.
    /// </summary>
    [CreateAssetMenu(fileName = "GameObjectChannelSO", menuName = "Events/GameObject Event Channel")]
    public class GameObjectEventChannelSO : GenericEventChannelSO<GameObject>
    {
    }
}

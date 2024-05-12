using LlamAcademy.ChickenDefense.Units.Llama.Behaviors;
using UnityEngine;

namespace LlamAcademy.ChickenDefense.EventChannels
{
    [CreateAssetMenu(fileName = "Llama Death Event Channel", menuName = "Events/Llama Death Event Channel")]
    public class LlamaDeathEventChannel : GenericEventChannelSO<LlamaDeathEvent>
    {

    }

    [System.Serializable]
    public struct LlamaDeathEvent
    {
        public Llama Llama;

        public LlamaDeathEvent(Llama llama)
        {
            Llama = llama;
        }
    }
}

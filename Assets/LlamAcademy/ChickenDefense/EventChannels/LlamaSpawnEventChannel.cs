using LlamAcademy.ChickenDefense.Units.Llama.Behaviors;
using UnityEngine;

namespace LlamAcademy.ChickenDefense.EventChannels
{
    [CreateAssetMenu(fileName = "Llama Spawn Event Channel", menuName = "Events/Llama Spawn Event Channel")]
    public class LlamaSpawnEventChannel : GenericEventChannelSO<LlamaSpawnEvent>
    {

    }

    [System.Serializable]
    public struct LlamaSpawnEvent
    {
        public Llama Llama;

        public LlamaSpawnEvent(Llama llama)
        {
            Llama = llama;
        }
    }
}

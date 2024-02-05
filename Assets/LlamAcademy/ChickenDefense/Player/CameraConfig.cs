using UnityEngine;

namespace LlamAcademy.ChickenDefense.Player
{
    [System.Serializable]
    public class CameraMoveConfig
    {
        public float BottomSafePercentage = 0.2f;
        [Header("Edge Panning")]
        public bool EnableEdgePan = true;
        public float MousePanSpeed = 5;
        public float EdgePanWidth = 50;
        [Header("Keyboard Panning")]
        public float KeyboardPanSpeed = 10;
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

namespace LlamAcademy.ChickenDefense.Player
{
    [System.Serializable]
    public class CameraMoveConfig
    {
        [Header("Edge Panning")]
        public bool EnableEdgePan = true;
        public float MousePanSpeed = 5;
        public float EdgePanWidth = 50;
        [Header("Keyboard Panning")]
        public float KeyboardPanSpeed = 10;

        public void HandlePanning(Vector2 mousePosition, Rigidbody virtualCameraTarget)
        {
            if (Mouse.current.leftButton.isPressed)
            {
                virtualCameraTarget.velocity = Vector3.zero;
                return;
            }
            Vector2 moveAmount = Vector2.zero;
            
            if (Keyboard.current.upArrowKey.isPressed)
            {
                moveAmount.y += KeyboardPanSpeed;
            }

            if (Keyboard.current.downArrowKey.isPressed)
            {
                moveAmount.y -= KeyboardPanSpeed;
            }

            if (Keyboard.current.leftArrowKey.isPressed)
            {
                moveAmount.x -= KeyboardPanSpeed;
            }

            if (Keyboard.current.rightArrowKey.isPressed)
            {
                moveAmount.x += KeyboardPanSpeed;
            }

            if (!EnableEdgePan)
            {
                return;
            }

            if (mousePosition.x < EdgePanWidth)
            {
                moveAmount.x = -MousePanSpeed;
            }
            else if (mousePosition.x > Screen.width - EdgePanWidth)
            {
                moveAmount.x = +MousePanSpeed;
            }

            if (mousePosition.y < EdgePanWidth)
            {
                moveAmount.y = -MousePanSpeed;
            }
            else if (mousePosition.y > Screen.height - EdgePanWidth)
            {
                moveAmount.y = +MousePanSpeed;
            }

            virtualCameraTarget.velocity = new Vector3(moveAmount.x, 0, moveAmount.y);
        }

    }
}
using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Events;
using UnityEngine;

namespace LlamAcademy.ChickenDefense.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private InputHandler[] InputHandlers;
        [SerializeField] private ActiveInputTarget DefaultTarget = ActiveInputTarget.Units;

        private void Start()
        {
            HandleInputModeChanged(new InputModeChangedEvent(DefaultTarget));
        }

        private void OnEnable()
        {
            Bus<InputModeChangedEvent>.OnEvent += HandleInputModeChanged;
        }

        private void OnDisable()
        {
            Bus<InputModeChangedEvent>.OnEvent -= HandleInputModeChanged;
        }

        private void HandleInputModeChanged(InputModeChangedEvent @event)
        {
            foreach (InputHandler handler in InputHandlers)
            {
                handler.Handler.enabled = @event.NewTarget == handler.InputTarget;
            }
        }
    }
}

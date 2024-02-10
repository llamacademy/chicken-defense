using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Events;
using UnityEngine;

namespace LlamAcademy.ChickenDefense.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private InputHandler[] InputHandlers;
        [SerializeField] private ActiveInputTarget DefaultTarget = ActiveInputTarget.Units;
        private EventBinding<InputModeChangedEvent> ChangeEvent;

        private void Awake()
        {
            ChangeEvent = new EventBinding<InputModeChangedEvent>(HandleInputModeChanged);
        }

        private void Start()
        {
            HandleInputModeChanged(new InputModeChangedEvent(DefaultTarget));
        }

        private void OnEnable()
        {
            Bus<InputModeChangedEvent>.Register(ChangeEvent);
        }

        private void OnDisable()
        {
            Bus<InputModeChangedEvent>.Unregister(ChangeEvent);
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
using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Events;
using LlamAcademy.ChickenDefense.Units;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace LlamAcademy.ChickenDefense.Player
{
    [RequireComponent(typeof(Camera))]
    public class PlayerBuildingController : MonoBehaviour
    {
        [SerializeField] private CameraMoveConfig CameraMoveConfig;
        [SerializeField] private Rigidbody VirtualCameraTarget;
        [SerializeField] private LayerMask PlacementLayers;
        
        private GameObject PlacementGhost;
        private Camera Camera;
        private UnitSO ActiveUnit;

        private EventBinding<UnitSelectedToPlaceEvent> UnitSelectedEventBinding;

        private void Awake()
        {
            Camera = GetComponent<Camera>();
            UnitSelectedEventBinding = new EventBinding<UnitSelectedToPlaceEvent>(HandleUnitSelected);
            Bus<UnitSelectedToPlaceEvent>.Register(UnitSelectedEventBinding);
        }

        private void Update()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray cameraRay = Camera.ScreenPointToRay(mousePosition);
            
            CameraMoveConfig.HandlePanning(mousePosition, VirtualCameraTarget);

            if (ActiveUnit != null && Physics.Raycast(
                    cameraRay,
                    out RaycastHit hit,
                    float.MaxValue,
                    PlacementLayers))
            {
                PlacementGhost.transform.position = hit.point;

                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    Instantiate(ActiveUnit.Prefab, PlacementGhost.transform.position, PlacementGhost.transform.rotation);
                    Destroy(PlacementGhost);
                    ActiveUnit = null;
                    Bus<InputModeChangedEvent>.Raise(new InputModeChangedEvent(ActiveInputTarget.Units));
                }
            }
        }

        private void HandleUnitSelected(UnitSelectedToPlaceEvent @event)
        {
            Destroy(PlacementGhost);
            Vector3 spawnLocation = Vector3.zero;
            
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray cameraRay = Camera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(
                    cameraRay,
                    out RaycastHit hit,
                    float.MaxValue,
                    PlacementLayers))
            {
                spawnLocation = hit.point;
            }
                
            PlacementGhost = Instantiate(@event.Unit.PlacementGhostPrefab, spawnLocation, quaternion.Euler(0, Random.Range(0, 359), 0));
            ActiveUnit = @event.Unit;
        }
    }
}
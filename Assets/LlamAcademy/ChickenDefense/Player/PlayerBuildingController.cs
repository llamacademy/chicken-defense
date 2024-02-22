using System.Collections.Generic;
using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Events;
using LlamAcademy.ChickenDefense.Units;
using LlamAcademy.ChickenDefense.Units.Chicken.Behaviors;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace LlamAcademy.ChickenDefense.Player
{
    public class PlayerBuildingController : MonoBehaviour
    {
        [SerializeField] private CameraMoveConfig CameraMoveConfig;
        [SerializeField] private Rigidbody VirtualCameraTarget;
        [SerializeField] private LayerMask PlacementLayers;
        [SerializeField] private Camera Camera;
        
        private GameObject PlacementGhost;
        private UnitSO ActiveUnit;

        private EventBinding<UnitSelectedToPlaceEvent> UnitSelectedEventBinding;
        private EventBinding<EggSpawnEvent> EggSpawnEventBinding;
        private EventBinding<EggRemovedEvent> EggRemovedEventBinding;
        private List<Egg> Eggs = new();

        private NavMeshQueryFilter QueryFilter;
        private static readonly int COLOR_PROPERTY = Shader.PropertyToID("_Tint");
        private static readonly Color BASE_COLOR = new(0.407f, 0.741f, 0.988f, 0.466f);

        private void Awake()
        {
            UnitSelectedEventBinding = new EventBinding<UnitSelectedToPlaceEvent>(HandleUnitSelected);
            Bus<UnitSelectedToPlaceEvent>.Register(UnitSelectedEventBinding);

            EggSpawnEventBinding = new EventBinding<EggSpawnEvent>(HandleEggSpawn);
            EggRemovedEventBinding = new EventBinding<EggRemovedEvent>(HandleEggRemoved);
            Bus<EggSpawnEvent>.Register(EggSpawnEventBinding);
            Bus<EggRemovedEvent>.Register(EggRemovedEventBinding);
        }

        private void Update()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray cameraRay = Camera.ScreenPointToRay(mousePosition);

            CameraMoveConfig.HandlePanning(mousePosition, VirtualCameraTarget);

            if (ActiveUnit != null)
            {
                SetGhostColorAndPosition(cameraRay);

                if (Mouse.current.leftButton.wasReleasedThisFrame && !EventSystem.current.IsPointerOverGameObject())
                {
                    Instantiate(ActiveUnit.Prefab, PlacementGhost.transform.position,
                        PlacementGhost.transform.rotation);
                    for (int i = 0; i < ActiveUnit.ResourceCost.Cost; i++)
                    {
                        Eggs[0].gameObject.SetActive(false);
                    }

                    Destroy(PlacementGhost);
                    ActiveUnit = null;
                    Bus<InputModeChangedEvent>.Raise(new InputModeChangedEvent(ActiveInputTarget.Units));
                }
            }
        }

        private void SetGhostColorAndPosition(Ray cameraRay)
        {
            if (Physics.Raycast(
                    cameraRay,
                    out RaycastHit hit,
                    float.MaxValue,
                    PlacementLayers) &&
                NavMesh.SamplePosition(hit.point, out NavMeshHit navMeshHit, 0.25f, QueryFilter))
            {
                foreach (Renderer renderer in PlacementGhost.GetComponentsInChildren<Renderer>())
                {
                    renderer.material.SetColor(COLOR_PROPERTY, BASE_COLOR);
                }
            }
            else
            {
                foreach (Renderer renderer in PlacementGhost.GetComponentsInChildren<Renderer>())
                {
                    renderer.material.SetColor(COLOR_PROPERTY, Color.red);
                }
            }

            PlacementGhost.transform.position = hit.point;
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

            PlacementGhost = Instantiate(@event.Unit.PlacementGhostPrefab, spawnLocation,
                quaternion.Euler(0, Random.Range(0, 359), 0));
            NavMeshAgent agent = PlacementGhost.GetComponent<NavMeshAgent>();
            QueryFilter = new NavMeshQueryFilter()
            {
                agentTypeID = agent.agentTypeID,
                areaMask = agent.areaMask
            };
            ActiveUnit = @event.Unit;
        }

        private void HandleEggSpawn(EggSpawnEvent @event)
        {
            Eggs.Add(@event.Egg);
        }

        private void HandleEggRemoved(EggRemovedEvent @event)
        {
            foreach (Egg egg in @event.Eggs)
            {
                Eggs.Remove(egg);
            }
        }
    }
}
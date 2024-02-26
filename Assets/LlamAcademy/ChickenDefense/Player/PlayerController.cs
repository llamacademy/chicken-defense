using System.Collections.Generic;
using System.Linq;
using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Events;
using LlamAcademy.ChickenDefense.Units;
using LlamAcademy.ChickenDefense.Units.Llama.Behaviors;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace LlamAcademy.ChickenDefense.Player
{
    [DefaultExecutionOrder(1)]
    public class PlayerController : MonoBehaviour
    {
        [Header("Camera Configuration")] 
        [SerializeField] private CameraMoveConfig CameraConfig = new();

        [SerializeField] private Rigidbody VirtualCameraTarget;
        [SerializeField] private RectTransform SelectionBox;
        [SerializeField] private float MouseDragDelay = 0.1f;

        [Space] [SerializeField] private Renderer ClickIcon;
        
        [Header("Layers")] 
        [SerializeField] private LayerMask SelectableLayers;
        [SerializeField] private LayerMask CommandTargetLayers;

        [SerializeField]
        private Camera Camera;

        private Vector2 StartMousePosition;
        private float MouseDownTime;
        [SerializeField]
        private List<AbstractUnit> AliveUnits = new(400);
        private HashSet<AbstractUnit> SelectedUnits = new(12);
        private HashSet<AbstractUnit> AddedUnits = new(12);
        private HashSet<AbstractUnit> RemovedUnits = new(12);

        private EventBinding<UnitSpawnEvent> UnitSpawnEventBinding;
        private EventBinding<UnitDeathEvent> UnitDeathEventBinding;
        private static readonly int CLICK_TIME_PROPERTY = Shader.PropertyToID("_ClickTime");

        private void Awake()
        {
            UnitSpawnEventBinding = new EventBinding<UnitSpawnEvent>(OnUnitSpawn);
            UnitDeathEventBinding = new EventBinding<UnitDeathEvent>(OnUnitDeath);
            Bus<UnitSpawnEvent>.Register(UnitSpawnEventBinding);
            Bus<UnitDeathEvent>.Register(UnitDeathEventBinding);

            VirtualCameraTarget.maxLinearVelocity =
                Mathf.Max(CameraConfig.KeyboardPanSpeed, CameraConfig.MousePanSpeed);
        }

        private void OnUnitSpawn(UnitSpawnEvent @event)
        {
            if (@event.Unit is Llama)
            {
                AliveUnits.Add(@event.Unit);                
            }
        }

        private void OnUnitDeath(UnitDeathEvent @event)
        {
            if (!AliveUnits.Contains(@event.Unit))
            {
                return;
            }
            
            AliveUnits.Remove(@event.Unit);
            SelectedUnits.Remove(@event.Unit);
        }

        private void Update()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray cameraRay = Camera.ScreenPointToRay(mousePosition);

            HandleDragSelect(cameraRay, mousePosition);
            CameraConfig.HandlePanning(mousePosition, VirtualCameraTarget);
            HandleIssuingCommands(cameraRay);
        }

        private void HandleDragSelect(Ray cameraRay, Vector2 mousePosition)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject())
            {
                SelectionBox.gameObject.SetActive(true);
                StartMousePosition = mousePosition;
                MouseDownTime = Time.time;
                SelectionBox.sizeDelta = Vector2.zero;
            }
            else if (Mouse.current.leftButton.isPressed && !Mouse.current.leftButton.wasPressedThisFrame)
            {
                Bounds selectionBoxBounds = ResizeSelectionBox(mousePosition);
                if (Mouse.current.delta.magnitude < 1)
                {
                    return;
                }
                foreach (AbstractUnit availableUnit in AliveUnits)
                {
                    Vector2 unitPosition = Camera.WorldToScreenPoint(availableUnit.transform.position);
                    if (selectionBoxBounds.Contains(unitPosition))
                    {
                        availableUnit.Select();
                        AddedUnits.Add(availableUnit);
                        RemovedUnits.Remove(availableUnit);
                    }
                    else
                    {
                        availableUnit.Deselect();
                        AddedUnits.Remove(availableUnit);
                        if (SelectedUnits.Contains(availableUnit))
                        {
                            RemovedUnits.Add(availableUnit);
                        }
                    }
                }
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                if (EventSystem.current.IsPointerOverGameObject() && !SelectionBox.gameObject.activeSelf)
                {
                    return;
                }
                
                SelectionBox.gameObject.SetActive(false);
                if (MouseDownTime + MouseDragDelay > Time.time || AddedUnits.Count == 0 && !EventSystem.current.IsPointerOverGameObject())
                {
                    HandleLeftClick(cameraRay);
                }
                else 
                {
                    foreach (AbstractUnit unit in AddedUnits)
                    {
                        SelectedUnits.Add(unit);
                    }

                    if (AddedUnits.Count > 0)
                    {
                        Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(AddedUnits.ToArray()));
                        AddedUnits.Clear();
                    }

                    foreach (AbstractUnit unit in RemovedUnits)
                    {
                        SelectedUnits.Remove(unit);
                    }

                    if (RemovedUnits.Count > 0)
                    {
                        Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(RemovedUnits.ToArray()));
                        RemovedUnits.Clear();
                    }
                }
            }
        }
        
        private void HandleLeftClick(Ray cameraRay)
        {
            if (Physics.Raycast(
                    cameraRay,
                    out RaycastHit hit,
                    float.MaxValue,
                    SelectableLayers
                ) && hit.transform.TryGetComponent(out AbstractUnit unit))
            {
                if (!Keyboard.current.leftShiftKey.isPressed && !Keyboard.current.rightShiftKey.isPressed)
                {
                    DeselectAllUnits(SelectedUnits);
                }

                SelectedUnits.Add(unit);
                unit.Select();
                Bus<UnitSelectedEvent>.Raise(new UnitSelectedEvent(unit));
            }
            else
            {
                DeselectAllUnits(SelectedUnits);
            }
        }
        
        private void DeselectAllUnits(HashSet<AbstractUnit> unitsToDeselect)
        {
            foreach (AbstractUnit selectable in unitsToDeselect)
            {
                selectable.Deselect();
            }

            Bus<UnitDeselectedEvent>.Raise(new UnitDeselectedEvent(unitsToDeselect.ToArray()));
            unitsToDeselect.Clear();
        }

        private Bounds ResizeSelectionBox(Vector2 mousePosition)
        {
            float width = mousePosition.x - StartMousePosition.x;
            float height = mousePosition.y - StartMousePosition.y;

            SelectionBox.anchoredPosition = StartMousePosition + new Vector2(width / 2, height / 2);
            SelectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));

            return new Bounds(SelectionBox.anchoredPosition, SelectionBox.sizeDelta);
        }

        private void HandleIssuingCommands(Ray cameraRay)
        {
            if (Mouse.current.rightButton.wasReleasedThisFrame && SelectedUnits.Count > 0
                && Physics.Raycast(
                    cameraRay,
                    out RaycastHit hit,
                    float.MaxValue,
                    CommandTargetLayers))
            {
                ClickIcon.transform.position = hit.point + Vector3.up * 0.01f;
                ClickIcon.material.SetFloat(CLICK_TIME_PROPERTY, Time.time);

                foreach (AbstractUnit unit in SelectedUnits)
                {
                    if (hit.collider.TryGetComponent(out IDamageable damageable))
                    {
                        unit.Attack(damageable);
                    }
                    else
                    {
                        unit.MoveTo(hit.point);    
                    }
                }
            }
        }
    }
}
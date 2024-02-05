using System.Collections.Generic;
using System.Linq;
using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Events;
using LlamAcademy.ChickenDefense.Units;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LlamAcademy.ChickenDefense.Player
{
    [RequireComponent(typeof(Camera))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Camera Configuration")] [SerializeField]
        private CameraMoveConfig CameraConfig = new();

        [SerializeField] private Rigidbody VirtualCameraTarget;
        [SerializeField] private RectTransform SelectionBox;
        [SerializeField] private float MouseDragDelay = 0.1f;

        [Header("Layers")] [SerializeField] private LayerMask SelectableLayers;
        [SerializeField] private LayerMask CommandTargetLayers;


        private Camera Camera;

        private Vector2 StartMousePosition;
        private float MouseDownTime;
        [SerializeField]
        private List<AbstractUnit> AliveUnits = new List<AbstractUnit>(400);
        private HashSet<AbstractUnit> SelectedUnits = new(12);
        private HashSet<AbstractUnit> AddedUnits = new(12);
        private HashSet<AbstractUnit> RemovedUnits = new(12);

        private EventBinding<UnitSpawnEvent> UnitSpawnEventBinding;
        private EventBinding<UnitDeathEvent> UnitDeathEventBinding;

        private void Awake()
        {
            Camera = GetComponent<Camera>();

            UnitSpawnEventBinding = new EventBinding<UnitSpawnEvent>(OnUnitSpawn);
            UnitDeathEventBinding = new EventBinding<UnitDeathEvent>(OnUnitDeath);
            Bus<UnitSpawnEvent>.Register(UnitSpawnEventBinding);
            Bus<UnitDeathEvent>.Register(UnitDeathEventBinding);

            VirtualCameraTarget.maxLinearVelocity =
                Mathf.Max(CameraConfig.KeyboardPanSpeed, CameraConfig.MousePanSpeed);
        }

        private void OnUnitSpawn(UnitSpawnEvent @event)
        {
            AliveUnits.Add(@event.Unit);
        }

        private void OnUnitDeath(UnitDeathEvent @event)
        {
            AliveUnits.Add(@event.Unit);
            SelectedUnits.Remove(@event.Unit);
        }

        private void Update()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray cameraRay = Camera.ScreenPointToRay(mousePosition);

            HandlePanning(mousePosition);
            HandleDragSelect(cameraRay, mousePosition);
            HandleIssuingCommands(cameraRay, mousePosition);
        }

        private void HandlePanning(Vector2 mousePosition)
        {
            if (!CameraConfig.EnableEdgePan)
            {
                return;
            }

            Vector2 moveAmount = Vector2.zero;
            if (mousePosition.x < CameraConfig.EdgePanWidth)
            {
                moveAmount.x = -CameraConfig.MousePanSpeed;
            }
            else if (mousePosition.x > Screen.width - CameraConfig.EdgePanWidth)
            {
                moveAmount.x = +CameraConfig.MousePanSpeed;
            }

            if (mousePosition.y < CameraConfig.EdgePanWidth)
            {
                moveAmount.y = -CameraConfig.MousePanSpeed;
            }
            else if (mousePosition.y > Screen.height - CameraConfig.EdgePanWidth)
            {
                moveAmount.y = +CameraConfig.MousePanSpeed;
            }

            if (Keyboard.current.upArrowKey.isPressed)
            {
                moveAmount.y += CameraConfig.KeyboardPanSpeed;
            }

            if (Keyboard.current.downArrowKey.isPressed)
            {
                moveAmount.y -= CameraConfig.KeyboardPanSpeed;
            }

            if (Keyboard.current.leftArrowKey.isPressed)
            {
                moveAmount.x -= CameraConfig.KeyboardPanSpeed;
            }

            if (Keyboard.current.rightArrowKey.isPressed)
            {
                moveAmount.x += CameraConfig.KeyboardPanSpeed;
            }

            VirtualCameraTarget.velocity = new Vector3(moveAmount.x, 0, moveAmount.y);
        }


        private void HandleDragSelect(Ray cameraRay, Vector2 mousePosition)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                SelectionBox.gameObject.SetActive(true);
                StartMousePosition = mousePosition;
                MouseDownTime = Time.time;
                SelectionBox.sizeDelta = Vector2.zero;
            }
            else if (Mouse.current.leftButton.isPressed)
            {
                Bounds selectionBoxBounds = ResizeSelectionBox(mousePosition);
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
                SelectionBox.gameObject.SetActive(false);
                if (MouseDownTime + MouseDragDelay > Time.time || AddedUnits.Count == 0)
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

                    
                    DeselectAllUnits(RemovedUnits);
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

        private void ClearSelectedUnits()
        {
            foreach (AbstractUnit unit in SelectedUnits)
            {
                ((ISelectable)unit).Deselect();
            }

            SelectedUnits.Clear();
        }

        private void HandleIssuingCommands(Ray cameraRay, Vector2 mousePosition)
        {
            if (Mouse.current.rightButton.wasReleasedThisFrame
                && Physics.Raycast(
                    cameraRay,
                    out RaycastHit hit,
                    float.MaxValue,
                    CommandTargetLayers))
            {
                foreach (AbstractUnit unit in SelectedUnits)
                {
                    unit.MoveTo(hit.point);
                    // handle attack or move, only 2 options so way easier than RTS
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Events;
using LlamAcademy.ChickenDefense.Player;
using LlamAcademy.ChickenDefense.UI.Components;
using LlamAcademy.ChickenDefense.Units;
using UnityEngine;
using UnityEngine.UIElements;

namespace LlamAcademy.ChickenDefense.UI
{
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(UIDocument))]
    public class RuntimeUI : MonoBehaviour
    {
        [SerializeField] private GameObject[] ObjectsToActivateOnPlay;
        [SerializeField] private Camera MinimapCamera;
        [SerializeField] private Transform VirtualCameraTarget;
        [SerializeField] private ResourceCostSO ResourceCost;
        [SerializeField] private UnitSO ChickenSO;
        [SerializeField] private UnitSO[] BuildableUnits;
        private UIDocument UI;
        private VisualElement Footer;
        private LabeledIcon Resources;
        private VisualElement UnitContainer;
        private VisualElement Minimap;
        
        private List<UnitPopulationLabeledIcon> PopulationLabeledIcons = new();

        private int CurrentEggs;
        private bool MouseDownOnMinimap;
        private LayerMask FloorLayer;
        
        private EventBinding<UnitSpawnEvent> SpawnEventBinding;
        private EventBinding<UnitDeathEvent> DieEventBinding;
        private EventBinding<EggSpawnEvent> EggSpawnBinding;
        private EventBinding<EggRemovedEvent> LostEggBinding;
        
        private void Awake()
        {
            UI = GetComponent<UIDocument>();

            VisualElement root = UI.rootVisualElement.Q("root");
            VisualElement runtimeUI = UI.rootVisualElement.Q("runtime-ui");
            runtimeUI.SetEnabled(false);
            MainMenu mainMenu = new (ObjectsToActivateOnPlay, () =>
            {
                MainMenu menu = root.Q<MainMenu>();
                menu.RemoveFromClassList("maximize");
                menu.SetEnabled(false);
            });
            mainMenu.AddToClassList("maximize");
            root.Add(mainMenu);
            
            SetupMinimapClickConfig();
            BuildUnitUI();
            BuildPopulationAndResourceUI();
        }

        private void Start()
        {
            SpawnEventBinding = new EventBinding<UnitSpawnEvent>(OnSpawnUnit);
            DieEventBinding = new EventBinding<UnitDeathEvent>(OnUnitDeath);
            EggSpawnBinding = new EventBinding<EggSpawnEvent>(OnSpawnEgg);
            LostEggBinding = new EventBinding<EggRemovedEvent>(OnLoseEgg);
            Bus<UnitSpawnEvent>.Register(SpawnEventBinding);
            Bus<UnitDeathEvent>.Register(DieEventBinding);
            Bus<EggSpawnEvent>.Register(EggSpawnBinding);
            Bus<EggRemovedEvent>.Register(LostEggBinding);
        }

        private void SetupMinimapClickConfig()
        {
            Minimap = UI.rootVisualElement.Q("minimap");
            FloorLayer = LayerMask.GetMask("Floor");
            Minimap.RegisterCallback<MouseDownEvent>(HandleMinimapMouseDown);
            Minimap.RegisterCallback<MouseMoveEvent>(HandleMinimapMouseMove);
            Minimap.RegisterCallback<MouseUpEvent>(HandleMinimapMouseUp);
            Minimap.RegisterCallback<MouseLeaveEvent>(HandleMinimapMouseLeave);
        }

        private void HandleMinimapMouseDown(MouseDownEvent evt)
        {
            MouseDownOnMinimap = true;

            MoveVirtualCameraTarget(evt.mousePosition);
        }

        private void HandleMinimapMouseMove(MouseMoveEvent evt)
        {
            if (MouseDownOnMinimap)
            {
                MoveVirtualCameraTarget(evt.mousePosition);
            }
        }

        private void HandleMinimapMouseLeave(MouseLeaveEvent evt)
        {
            MouseDownOnMinimap = false;
        }

        private void HandleMinimapMouseUp(MouseUpEvent evt)
        {
            MouseDownOnMinimap = false;
        }
        
        private void MoveVirtualCameraTarget(Vector2 mousePosition)
        {
            // convert screen mouse position to "minimap screen" position
            float widthMultiplier = (MinimapCamera.scaledPixelWidth / Minimap.layout.width);
            float heightMultiplier = (MinimapCamera.scaledPixelHeight / Minimap.layout.width);
            Vector2 convertedMousePosition = new(
                mousePosition.x * widthMultiplier,
                (Screen.height - mousePosition.y) * heightMultiplier // mousePosition.y is tied to top left instead of bottom left
            );
            
            Ray cameraRay = MinimapCamera.ScreenPointToRay(convertedMousePosition);
            if (Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, FloorLayer))
            {
                VirtualCameraTarget.position = hit.point;
            }
        }

        private void BuildPopulationAndResourceUI()
        {
            VisualElement headerContainer = UI.rootVisualElement.Q("runtime-ui__header");
            Resources = new LabeledIcon(ResourceCost.Icon, "0");
            headerContainer.Add(Resources);

            UnitPopulationLabeledIcon labeledIcon = new (ChickenSO, "0");
            PopulationLabeledIcons.Add(labeledIcon);
            headerContainer.Add(labeledIcon);
            
            foreach (UnitSO buildableUnit in BuildableUnits)
            {
                labeledIcon = new UnitPopulationLabeledIcon(buildableUnit, "0");
                
                headerContainer.Add(labeledIcon);
                PopulationLabeledIcons.Add(labeledIcon);
            }
            
            labeledIcon.AddToClassList("padding-right-sm");
        }
        
        private void BuildUnitUI()
        {
            UnitContainer = UI.rootVisualElement.Q("runtime-ui__footer");
            
            Array.Sort(BuildableUnits, (a, b) => Mathf.FloorToInt(a.ResourceCost.Cost - b.ResourceCost.Cost));
            foreach (UnitSO buildableUnit in BuildableUnits)
            {
                VisualElement clickableUnit = new UnitIcon(buildableUnit);
                clickableUnit.AddToClassList("padding-right-sm");
                clickableUnit.RegisterCallback<ClickEvent>((_) => HandleUnitClick(buildableUnit));
                clickableUnit.SetEnabled(CurrentEggs >= buildableUnit.ResourceCost.Cost);
                UnitContainer.Add(clickableUnit);
            }
        }

        private void HandleUnitClick(UnitSO unit)
        {
            if (CurrentEggs >= unit.ResourceCost.Cost)
            {
                Bus<UnitSelectedToPlaceEvent>.Raise(new UnitSelectedToPlaceEvent(unit));
                Bus<InputModeChangedEvent>.Raise(new InputModeChangedEvent(ActiveInputTarget.Building));                
            }
        }

        private void OnSpawnUnit(UnitSpawnEvent @event)
        {
            UnitPopulationLabeledIcon labeledIcon =
                PopulationLabeledIcons.FirstOrDefault(labeledIcon => @event.Unit.Unit == labeledIcon.Unit);
            if (labeledIcon != null)
            {
                labeledIcon.Count++;
            }
        }
        
        private void OnUnitDeath(UnitDeathEvent @event)
        {
            UnitPopulationLabeledIcon labeledIcon =
                PopulationLabeledIcons.FirstOrDefault(labeledIcon => @event.Unit.Unit == labeledIcon.Unit);
            if (labeledIcon != null)
            {
                labeledIcon.Count--;
            }
        }

        private void OnSpawnEgg(EggSpawnEvent @event)
        {
            CurrentEggs++;
            Resources.SetText(CurrentEggs.ToString());
            UpdateBuildableUnitEnabledStatus();            
        }

        private void UpdateBuildableUnitEnabledStatus()
        {
            int i = 0;
            foreach (VisualElement child in UnitContainer.Children())
            {
                child.SetEnabled(CurrentEggs >= BuildableUnits[i].ResourceCost.Cost);
                i++;
            }
        }

        private void OnLoseEgg(EggRemovedEvent @event)
        {
            CurrentEggs -= @event.Eggs.Length;
            Resources.SetText(CurrentEggs.ToString());
            UpdateBuildableUnitEnabledStatus();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using LlamAcademy.ChickenDefense.AI;
using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Events;
using LlamAcademy.ChickenDefense.Player;
using LlamAcademy.ChickenDefense.UI.Components;
using LlamAcademy.ChickenDefense.UI.Screens;
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
        private VisualElement RuntimeUIElement;

        private List<UnitPopulationLabeledIcon> PopulationLabeledIcons = new();

        private int CurrentEggs;
        private bool MouseDownOnMinimap;
        private LayerMask FloorLayer;

        private float StartTime;
        private Difficulty Difficulty;

        private void Awake()
        {
            UI = GetComponent<UIDocument>();

            VisualElement root = UI.rootVisualElement.Q("root");
            RuntimeUIElement = UI.rootVisualElement.Q("runtime-ui");
            RuntimeUIElement.AddToClassList("disabled");
            MainMenu mainMenu = new(ObjectsToActivateOnPlay, () =>
            {
                MainMenu menu = root.Q<MainMenu>();
                menu.RemoveFromClassList("maximize");
                menu.AddToClassList("disabled");
                StartTime = Time.time;
                Difficulty = menu.GameDifficulty;
            });
            mainMenu.AddToClassList("maximize");
            root.Add(mainMenu);

            SetupMinimapClickConfig();
            BuildUnitUI();
            BuildPopulationAndResourceUI();
        }

        private void OnEnable()
        {
            Bus<UnitSpawnEvent>.OnEvent += OnSpawnUnit;
            Bus<UnitDeathEvent>.OnEvent += OnUnitDeath;
            Bus<EggSpawnEvent>.OnEvent += OnSpawnEgg;
            Bus<EggRemovedEvent>.OnEvent += OnLoseEgg;
        }

        private void Start()
        {
            Bus<GameOverEvent>.OnEvent += HandleGameOver;
        }

        private void OnDisable()
        {
            Bus<UnitSpawnEvent>.OnEvent -= OnSpawnUnit;
            Bus<UnitDeathEvent>.OnEvent -= OnUnitDeath;
            Bus<EggSpawnEvent>.OnEvent -= OnSpawnEgg;
            Bus<EggRemovedEvent>.OnEvent -= OnLoseEgg;
            Bus<GameOverEvent>.OnEvent -= HandleGameOver;
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

        private void HandleGameOver(GameOverEvent evt)
        {
            if (UI == null || UI.rootVisualElement == null)
            {
                return; // in editor, when quitting this throws a MissingReferenceException due to order of object destruction
            }
            foreach (GameObject go in ObjectsToActivateOnPlay)
            {
                if (go != null)
                {
                    go.SetActive(false);
                }
            }

            RuntimeUIElement.AddToClassList("disabled");

            EndGameScreen endGameScreen = new(Time.time - StartTime, Difficulty);
            endGameScreen.AddToClassList("maximize");
            UI.rootVisualElement.Add(endGameScreen);
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
                (Screen.height - mousePosition.y) *
                heightMultiplier // mousePosition.y is tied to top left instead of bottom left
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

            UnitPopulationLabeledIcon labeledIcon = new(ChickenSO, "0");
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

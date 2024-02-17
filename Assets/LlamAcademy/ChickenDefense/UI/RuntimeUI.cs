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
        [SerializeField] private ResourceCostSO ResourceCost;
        [SerializeField] private UnitSO ChickenSO;
        [SerializeField] private UnitSO[] BuildableUnits;
        private UIDocument UI;
        private VisualElement Footer;
        private LabeledIcon Resources;
        private VisualElement UnitContainer;
        
        private List<UnitPopulationLabeledIcon> PopulationLabeledIcons = new();

        private int CurrentEggs;
        
        private EventBinding<UnitSpawnEvent> SpawnEventBinding;
        private EventBinding<UnitDeathEvent> DieEventBinding;
        private EventBinding<EggSpawnEvent> EggSpawnBinding;
        private EventBinding<EggRemovedEvent> LostEggBinding;
        
        private void Awake()
        {
            UI = GetComponent<UIDocument>();

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
        }
        
        private void BuildUnitUI()
        {
            UnitContainer = UI.rootVisualElement.Q("runtime-ui__footer");
            
            Array.Sort(BuildableUnits, (a, b) => Mathf.FloorToInt(a.ResourceCost.Cost - b.ResourceCost.Cost));
            foreach (UnitSO buildableUnit in BuildableUnits)
            {
                VisualElement clickableUnit = new UnitIcon(buildableUnit);
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
using System;
using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Events;
using LlamAcademy.ChickenDefense.Player;
using LlamAcademy.ChickenDefense.UI.Components;
using LlamAcademy.ChickenDefense.Units;
using UnityEngine;
using UnityEngine.UIElements;

namespace LlamAcademy.ChickenDefense.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class RuntimeUI : MonoBehaviour
    {
        [SerializeField] private UnitSO[] BuildableUnits;
        private UIDocument UI;
        private VisualElement Footer;
        private LabeledIcon Resources;
        private LabeledIcon Population;

        private EventBinding<UnitSpawnEvent> SpawnEventBinding;
        private EventBinding<UnitDeathEvent> DieEventBinding;
        
        private void Awake()
        {
            UI = GetComponent<UIDocument>();
        }

        private void Start()
        {
            SpawnEventBinding = new EventBinding<UnitSpawnEvent>(OnSpawnUnit);
            DieEventBinding = new EventBinding<UnitDeathEvent>(OnUnitDeath);
            Bus<UnitSpawnEvent>.Register(SpawnEventBinding);
            Bus<UnitDeathEvent>.Register(DieEventBinding);

            BuildUnitUI();
        }

        private void BuildUnitUI()
        {
            VisualElement unitContainer = UI.rootVisualElement.Q("runtime-ui__footer");
            
            Array.Sort(BuildableUnits, (a, b) => Mathf.FloorToInt(a.ResourceCost.Cost - b.ResourceCost.Cost));
            foreach (UnitSO buildableUnit in BuildableUnits)
            {
                VisualElement clickableUnit = new UnitIcon(buildableUnit);
                clickableUnit.RegisterCallback<ClickEvent>((_) => HandleUnitClick(buildableUnit));
                unitContainer.Add(clickableUnit);
            }
        }

        private void HandleUnitClick(UnitSO unit)
        {
            Bus<UnitSelectedToPlaceEvent>.Raise(new UnitSelectedToPlaceEvent(unit));
            Bus<InputModeChangedEvent>.Raise(new InputModeChangedEvent(ActiveInputTarget.Building));
        }

        private void OnSpawnUnit(UnitSpawnEvent @event)
        {
           
        }
        
        private void OnUnitDeath(UnitDeathEvent @event)
        {
            
        }
        
    }
}
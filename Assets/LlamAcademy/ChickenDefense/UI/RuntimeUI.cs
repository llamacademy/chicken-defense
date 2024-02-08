using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Events;
using LlamAcademy.ChickenDefense.UI.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace LlamAcademy.ChickenDefense.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class RuntimeUI : MonoBehaviour
    {
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
        }

        private void OnSpawnUnit(UnitSpawnEvent @event)
        {
           
        }
        
        private void OnUnitDeath(UnitDeathEvent @event)
        {
            
        }
        
    }
}
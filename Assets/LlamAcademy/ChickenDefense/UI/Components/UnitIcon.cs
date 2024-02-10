using LlamAcademy.ChickenDefense.Units;
using UnityEditor;
using UnityEngine.Rendering.UI;
using UnityEngine.UIElements;
using Label = System.Reflection.Emit.Label;

namespace LlamAcademy.ChickenDefense.UI.Components
{
    public class UnitIcon : VisualElement
    {
        public UnitSO Unit { get; private set; }
        
        public new class UxmlFactory : UxmlFactory<LabeledIcon> {}
        public UnitIcon() {}

        private VisualElement Icon => this.Q<VisualElement>("icon");

        public UnitIcon(UnitSO unit)
        {
            Unit = unit;
            Setup(unit);
        }

        public void Setup(UnitSO unit)
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/LlamAcademy/ChickenDefense/UI/Components/UnitIcon.uxml");
            asset.CloneTree(this);
            Icon.style.backgroundImage = new StyleBackground(unit.UIIcon);
            LabeledIcon label = new(unit.ResourceCost.Icon, unit.ResourceCost.Cost.ToString());
            Icon.Add(label);
        }
    }
}

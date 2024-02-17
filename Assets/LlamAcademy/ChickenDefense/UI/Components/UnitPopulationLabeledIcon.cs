using LlamAcademy.ChickenDefense.Units;
using UnityEngine.UIElements;

namespace LlamAcademy.ChickenDefense.UI.Components
{
    public class UnitPopulationLabeledIcon : LabeledIcon
    {
        public UnitSO Unit;

        private int _Count;
        public int Count
        {
            get => _Count;
            set
            {
                SetText(value.ToString());
                _Count = value;
            }
        }
        
        public new class UxmlFactory : UxmlFactory<UnitPopulationLabeledIcon> {}
        public UnitPopulationLabeledIcon() {}

        public UnitPopulationLabeledIcon(UnitSO unit, string label) : base(unit.UIIcon, label)
        {
            Unit = unit;
            SetIcon(Unit.UIIcon);
            SetText(label);
        }
    }
}
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LlamAcademy.ChickenDefense.UI.Components
{
    public class LabeledIcon : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<LabeledIcon> {}
        public LabeledIcon() {}

        private VisualElement Icon => this.Q<VisualElement>("icon");
        private Label Label => this.Q<Label>("label");

        public LabeledIcon(Texture2D icon, string label)
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/LlamAcademy/ChickenDefense/UI/Components/LabeledIcon.uxml");
            asset.CloneTree(this);
            Icon.style.backgroundImage = new StyleBackground(icon);
            Label.text = label;
        }

        public void SetText(string label)
        {
            Label.text = label;
        }

        public void SetIcon(Texture2D icon)
        {
            Icon.style.backgroundImage = new StyleBackground(icon);
        }
    }
}

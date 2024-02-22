using System;
using LlamAcademy.ChickenDefense.AI;
using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Events;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LlamAcademy.ChickenDefense.UI
{
    public class MainMenu : VisualElement
    {
        private GameObject[] ObjectsToActivateOnPlay;

        public new class UxmlFactory : UxmlFactory<MainMenu> {}

        public MainMenu()
        {
        }

        private Action OnClose;
        private VisualElement MenuRoot => this.Q("main-menu");
        private Button PlayButton => this.Q<Button>("play");
        private Button AboutButton => this.Q<Button>("about");
        private Button ExitButton => this.Q<Button>("exit");
        private Button ClosePopupButton => this.Q<Button>("close-popup");
        private Button GetCodeButton => this.Q<Button>("get-code");
        private Button ViewYouTubeButton => this.Q<Button>("view-youtube");
        private Button PatreonButton => this.Q<Button>("patreon");
        private VisualElement ChickenLabel => this.Q<VisualElement>("chicken");
        private VisualElement DefenseLabel => this.Q<VisualElement>("defense");
        private VisualElement Popup => this.Q<VisualElement>("popup");
        private EnumField Difficulty => this.Q<EnumField>("difficulty");

        public MainMenu(GameObject[] objectsToActivateOnPlay, Action onClose = null)
        {
            Setup(objectsToActivateOnPlay);
            OnClose = onClose;
        }
        
        public void Setup(GameObject[] objectsToActivateOnPlay)
        {
            ObjectsToActivateOnPlay = objectsToActivateOnPlay;
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/LlamAcademy/ChickenDefense/UI/MainMenu.uxml");
            asset.CloneTree(this);

            // delay invoking adding the class so the transition plays
            ChickenLabel.schedule.Execute(() => ChickenLabel.AddToClassList("active")).StartingIn(1000);
            DefenseLabel.schedule.Execute(() => DefenseLabel.AddToClassList("active")).StartingIn(1000);
            
            Popup.SetEnabled(false);
            
            PlayButton.RegisterCallback<ClickEvent>(HandlePlayClick);
            ExitButton.RegisterCallback<ClickEvent>(HandleExitClick);
            AboutButton.RegisterCallback<ClickEvent>(HandleCodeClick);
            ClosePopupButton.RegisterCallback<ClickEvent>(HandleClosePopupClick);
            GetCodeButton.RegisterCallback<ClickEvent>(HandleGetCodeClick);
            ViewYouTubeButton.RegisterCallback<ClickEvent>(HandleViewYouTubeClick);
            PatreonButton.RegisterCallback<ClickEvent>(HandlePatreonClick);
            Difficulty.RegisterCallback<ChangeEvent<string>>(HandleDifficultyChanged);
        }

        private void HandleDifficultyChanged(ChangeEvent<string> evt)
        {
            Bus<DifficultyChangedEvent>.Raise(new DifficultyChangedEvent(Enum.Parse<Difficulty>(evt.newValue)));
        }

        private void HandlePlayClick(ClickEvent _)
        {
            foreach (GameObject go in ObjectsToActivateOnPlay)
            {
                go.SetActive(true);
            }

            VisualElement runtimeUI = this.panel.visualTree.Q("runtime-ui");
            runtimeUI.SetEnabled(true);
            MenuRoot.SetEnabled(false);
            OnClose?.Invoke();
        }
        
        private void HandleExitClick(ClickEvent _)
        {
            Application.Quit();
        }
        
        private void HandleCodeClick(ClickEvent _)
        {
            Popup.SetEnabled(true);
            Popup.pickingMode = PickingMode.Position;
            foreach (VisualElement child in Popup.Children())
            {
                child.pickingMode = PickingMode.Position;
            }
        }
        
        private void HandleClosePopupClick(ClickEvent _)
        {
            Popup.SetEnabled(false);
            Popup.pickingMode = PickingMode.Ignore;
        }
        
        private void HandleGetCodeClick(ClickEvent _)
        {
            Application.OpenURL("https://github.com/llamacademy/chicken-defense");
        }
        
        private void HandleViewYouTubeClick(ClickEvent _)
        {
            Application.OpenURL("https://youtube.com/@llamacademy");
        }
        
        private void HandlePatreonClick(ClickEvent _)
        {
            Application.OpenURL("https://patreon.com/llamacademy");
        }
    }
}

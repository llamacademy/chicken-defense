using System;
using LlamAcademy.ChickenDefense.AI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace LlamAcademy.ChickenDefense.UI.Screens
{
    public class EndGameScreen : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<EndGameScreen> {}
        public EndGameScreen() {}

        private Button PlayAgainButton => this.Q<Button>("play-game");
        private Label ScoreLabel => this.Q<Label>("score");

        private const string TIME_FORMAT = @"hh\:mm\:ss";
        public EndGameScreen(float totalElapsedTime, Difficulty difficulty)
        {
            Setup(totalElapsedTime, difficulty);
        }

        public void Setup(float totalElapsedTime, Difficulty difficulty)
        {
            VisualTreeAsset asset = Resources.Load<VisualTreeAsset>("UI/EndGameScreen");
            asset.CloneTree(this);

            float highScore = PlayerPrefs.GetFloat(difficulty.ToString());
            if (highScore < totalElapsedTime)
            {
                PlayerPrefs.SetFloat(difficulty.ToString(), totalElapsedTime);
                highScore = totalElapsedTime;
            }
            TimeSpan timeSpan = TimeSpan.FromSeconds(totalElapsedTime);
            ScoreLabel.text = $"You lasted <b><color=#ff6c00>{timeSpan.ToString(TIME_FORMAT)}</color></b>!\r\nHigh Score: <b><color=#ff6c00>{TimeSpan.FromSeconds(highScore).ToString(TIME_FORMAT)}</color></b>\r\nDare to try again?";
            
            PlayAgainButton.RegisterCallback<ClickEvent>(PlayAgain);
        }

        private void PlayAgain(ClickEvent _)
        {
            SceneManager.LoadScene("Game");
        }
    }
}
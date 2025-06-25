using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestGame.Gameplay.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TestGame.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _mainMenuGroup;
        [SerializeField] private TMP_Text _highscore;
        [SerializeField] private GameObject _highscoreObject;

        [SerializeField] private CanvasGroup _winScreenGroup;
        [SerializeField] private CanvasGroup _loadingScreen;
        [SerializeField] private CanvasGroup _gameplayHUD;
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private WinScreen _winScreen;

        [SerializeField] private Button _startGameButton;


        private const string HighScoreSave = "highscore.json";
        private void Start()
        {
            _startGameButton.onClick.AddListener(StartGame);
            CheckHighScore();
        }


        private void CheckHighScore()
        {
            var highscoreTime = SaveSystem.Load<Highscore>(HighScoreSave).Time;
            if (!highscoreTime.Equals(float.MaxValue))
            {
                _highscoreObject.SetActive(true);
                int minutes = Mathf.FloorToInt(highscoreTime / 60f);
                int seconds = Mathf.FloorToInt(highscoreTime % 60f);
                _highscore.text = string.Format("Highscore {0:00}:{1:00}", minutes, seconds);
            }
            else
            {
                _highscoreObject.SetActive(false);
            }
        }

        public async void StartGame()
        {
            StartCoroutine(FadeCanvasGroup(_loadingScreen, 1, _animationDuration));
            await Task.Delay(1000);
            StartCoroutine(FadeCanvasGroup(_mainMenuGroup, 0, _animationDuration));
            GameController.Instance.StartGame();
            await Task.Delay(1000);
            StartCoroutine(FadeCanvasGroup(_loadingScreen, 0, _animationDuration));
            StartCoroutine(FadeCanvasGroup(_gameplayHUD, 1, _animationDuration));
        }

        public void Win()
        {
            StartCoroutine(FadeCanvasGroup(_gameplayHUD, 0, _animationDuration));
            StartCoroutine(FadeCanvasGroup(_winScreenGroup, 1, _animationDuration));
           
        }

        public void ResetToMainMenu()
        {
            StartCoroutine(FadeCanvasGroup(_mainMenuGroup, 1, _animationDuration));
            StartCoroutine(FadeCanvasGroup(_winScreenGroup, 0, _animationDuration));
            CheckHighScore();
        }

        public void DrawWinScreen(string text, ushort steps)
        {
            _winScreen.Draw(text, steps);
        }


        public IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float duration)
        {
            float startAlpha = canvasGroup.alpha;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha; // Ensure exact target at end
            canvasGroup.interactable = targetAlpha > 0.95f;
            canvasGroup.blocksRaycasts = targetAlpha > 0.95f;
        }
    }
}
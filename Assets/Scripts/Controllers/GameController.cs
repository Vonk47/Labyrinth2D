using JSAM;
using System.Collections;
using System.Threading.Tasks;
using TestGame.UI;
using UnityEngine;

namespace TestGame.Gameplay.Controllers
{
    //god object
    public class GameController : MonoSingleton<GameController>
    {
        [SerializeField] private TimeTracker _timeTracker;
        [SerializeField] private StepsTracker _stepsTracker;
        [SerializeField] private LabyrinthGenerator _generator;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private UIController _uiController;
        [SerializeField] private ParticleSystem _winConfetti;
        [SerializeField] private ParticleSystem _winConfetti2;
        [SerializeField] private ParticleSystem _winConfetti3;

        private const string SavePath = "highscore.json";

        private void Awake()
        {
            InitializeSingleton();
            Application.targetFrameRate = 60;

        }

        public IEnumerator Start()
        {
            // this guy need time to wait...
            yield return new WaitForSeconds(1);
            AudioManager.FadeMusicIn(AudioLibraryMusic.MusicFile, 1, true);
        }

        public void AddSteps()
        {
            _stepsTracker.AddStep();
        }

        public void StartGame()
        {
            _generator.StartGame();
            _timeTracker.StartTimer();
            _stepsTracker.ResetSteps();
            _playerController.EnablePlayer(true);
        }


        public async void Win()
        {

            AudioManager.PlaySound(AudioLibrarySounds.WinSounds);

            if (SaveSystem.Load<Highscore>(SavePath).Time > _timeTracker.ElapsedTime)
            {
                SaveSystem.Save<Highscore>(new Highscore(_timeTracker.ElapsedTime), SavePath);
            }

            _winConfetti.Play();
            _timeTracker.StopTimer();
            await Task.Delay(1000);
            _winConfetti2.Play();
            await Task.Delay(400);
            _winConfetti3.Play();
            await Task.Delay(1200);
            _uiController.DrawWinScreen(_timeTracker.ReturnCurrentTime, _stepsTracker.CurrentSteps);
            _uiController.Win();

            _timeTracker.ResetTimer();
            _playerController.EnablePlayer(false);
        }

        public void ReturnToMainMenu()
        {
            _uiController.ResetToMainMenu();
        }

    }
}
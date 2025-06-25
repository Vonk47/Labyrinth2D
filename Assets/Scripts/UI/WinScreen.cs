using TestGame.Gameplay.Controllers;
using TMPro;
using UnityEngine;


namespace TestGame.UI
{
    public class WinScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text _youWonDescription;

        private static string TextFormat = "You have escaped the labyrinth in: {0}, using {1} steps";

        public void Draw(string timeText, ushort steps)
        {
            _youWonDescription.text = string.Format(TextFormat, timeText, steps);
        }

        public void Trigger_Next()
        {
            GameController.Instance.ReturnToMainMenu();
        }
    }
}
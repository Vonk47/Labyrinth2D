using TMPro;
using UnityEngine;

namespace TestGame.UI
{
    public class StepsTracker : MonoBehaviour
    {
        [SerializeField] private TMP_Text _stepsText;
        private ushort _currentSteps;

        public ushort CurrentSteps => _currentSteps;

        public void AddStep()
        {
            _currentSteps++;
            Draw();
        }


        public void Draw()
        {
            _stepsText.text = _currentSteps.ToString();
        }

        public void ResetSteps()
        {
            _currentSteps = 0;
            Draw();
        }

    }
}
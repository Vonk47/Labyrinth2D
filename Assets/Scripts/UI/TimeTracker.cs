using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TestGame.UI
{
    public class TimeTracker : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timeText;

        private float _elapsedTime = 0f;
        public float ElapsedTime => _elapsedTime;
        private bool _isTracking = false;

        private void Update()
        {
            if (!_isTracking) return;

            _elapsedTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(_elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(_elapsedTime % 60f);
            _timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        public string ReturnCurrentTime => _timeText.text;

        public void StartTimer()
        {
            _elapsedTime = 0f;
            _isTracking = true;
        }

        public void StopTimer()
        {
            _isTracking = false;
        }

        public void ResetTimer()
        {
            _elapsedTime = 0f;
            _timeText.text = "00:00";
        }
    }
}
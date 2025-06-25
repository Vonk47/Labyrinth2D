using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestGame.UI
{
    public class PulseAnimation : MonoBehaviour
    {
        [SerializeField] private float _scaleFactor = 1.1f;
        [SerializeField] private float _duration = 0.5f;

        private Vector3 _originalScale;

        private void Start()
        {
            _originalScale = transform.localScale;
            StartCoroutine(PulseLoop());
        }

        private IEnumerator PulseLoop()
        {
            while (true)
            {
                yield return ScaleTo(_originalScale * _scaleFactor, _duration);
                yield return ScaleTo(_originalScale, _duration);
            }
        }

        private IEnumerator ScaleTo(Vector3 target, float duration)
        {
            Vector3 start = transform.localScale;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                transform.localScale = Vector3.Lerp(start, target, t);
                yield return null;
            }

            transform.localScale = target;
        }
    }
}
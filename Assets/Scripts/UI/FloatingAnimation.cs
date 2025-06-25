using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestGame.UI
{
    public class FloatingAnimation : MonoBehaviour
    {
        [SerializeField] private float _floatDistance = 10f;
        [SerializeField] private float _floatDuration = 1f;

        private RectTransform _rectTransform;
        private Vector2 _startPos;
        private Coroutine _floatAnimation;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _startPos = _rectTransform.anchoredPosition;
            _floatAnimation = StartCoroutine(FloatAnimation());
        }

        private IEnumerator FloatAnimation()
        {
            while (true)
            {
                yield return MoveTo(_startPos + Vector2.up * _floatDistance, _floatDuration);
                yield return MoveTo(_startPos, _floatDuration);
            }
        }

        private IEnumerator MoveTo(Vector2 target, float duration)
        {
            Vector2 initial = _rectTransform.anchoredPosition;
            float time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                _rectTransform.anchoredPosition = Vector2.Lerp(initial, target, t);
                yield return null;
            }
            _rectTransform.anchoredPosition = target;
        }
    }
}
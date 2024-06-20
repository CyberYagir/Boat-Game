using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Content.Scripts.Map.UI
{
    public class UIMoveIslandTimer : MonoBehaviour
    {
        public const float HOURS_IN_DAY = 86400;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text text;
        private float distance;
        private event Action OnEndCallback;


        public void Init()
        {
            canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }


        public void Show(float distance, Action onEndCallback)
        {
            this.OnEndCallback = onEndCallback;
            gameObject.SetActive(true);
            this.distance = distance;
            var timeSpan = TimeSpan.FromSeconds(distance * HOURS_IN_DAY);
            UpdateText(timeSpan);
            canvasGroup.DOFade(1, 0.2f).onComplete += OnShowed;
        }

        private void UpdateText(TimeSpan timeSpan)
        {
            text.text = timeSpan.Hours.ToString("00") + ":" + timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");
        }

        private void OnShowed()
        {
            DOVirtual.Float(1, 0, 10, OnTimeUpdating).onComplete += OnEndTime;
            OnTimeUpdating(0);
        }

        private void OnEndTime()
        {
            OnEndCallback?.Invoke();
            canvasGroup.DOFade(0, 0.2f).SetDelay(1f).onComplete += delegate { gameObject.SetActive(false); };
            OnEndCallback = null;
        }

        private void OnTimeUpdating(float value)
        {
            var timeSpan = TimeSpan.FromSeconds(distance * HOURS_IN_DAY * value);
            UpdateText(timeSpan);
        }
    }
}

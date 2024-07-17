using System;
using Content.Scripts.BoatGame.Services;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class AnimatedWindow : MonoBehaviour
    {
        [SerializeField] private GameObject window;
        [SerializeField] private bool canShowOnlySingle;

        private bool isOpen;
        private UIService uiServiceCached;

        public bool IsOpen => isOpen;

        public event Action<AnimatedWindow> OnOpen;
        public event Action<AnimatedWindow> OnClose;

        public void InitWindow(UIService uiServiceCached)
        {
            this.uiServiceCached = uiServiceCached;
        }

        public virtual void ShowWindow()
        {
            if (canShowOnlySingle)
            {
                if (uiServiceCached != null){
                    if (uiServiceCached.WindowManager.isAnyWindowOpened)
                    {
                        return;
                    }
                }
            }
            
            window.transform.DOKill();
            window.gameObject.SetActive(true);
            window.transform.localScale = Vector3.zero;
            window.transform.DOScale(Vector3.one, 0.2f);
            isOpen = true;
            OnOpen?.Invoke(this);
        }

        public virtual void CloseWindow()
        {
            window.transform.DOKill();
            window.transform.DOScale(Vector3.zero, 0.2f).onComplete += delegate
            {
                window.gameObject.SetActive(false);
                OnClosed();
            };
            isOpen = false;
        }

        public virtual void OnClosed()
        {
            OnClose?.Invoke(this);
        }
    }
}
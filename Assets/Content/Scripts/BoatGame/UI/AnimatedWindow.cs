using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.BoatGame.UI
{
    public class AnimatedWindow : MonoBehaviour
    {
        [SerializeField] private GameObject window;

        private bool isOpen;

        public bool IsOpen => isOpen;

        public virtual void ShowWindow()
        {
            window.transform.DOKill();
            window.gameObject.SetActive(true);
            window.transform.localScale = Vector3.zero;
            window.transform.DOScale(Vector3.one, 0.2f);
            isOpen = true;
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
            
        }
    }
}
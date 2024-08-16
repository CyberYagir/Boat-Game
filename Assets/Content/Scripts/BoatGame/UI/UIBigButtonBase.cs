using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.BoatGame.UI
{
    public class UIBigButtonBase : MonoBehaviour
    {
        [SerializeField] protected RectTransform button;
        [SerializeField] protected Image image;
        protected bool isPressed = false;

        public void ButtonDown()
        {
            if (isPressed) return;
            isPressed = true;
            button.DOScale(Vector3.one * 0.8f, 0.2f);
            image.DOFade(0.8f, 0.2f);
            OnButtonDown();
        }

        public virtual void OnButtonDown()
        {
        }

        public void ButtonUp()
        {
            if (!isPressed) return;
            isPressed = false;
            button.DOScale(Vector3.one, 0.2f);
            image.DOFade(1f, 0.2f);

            OnButtonUp();
        }

        public virtual void OnButtonUp()
        {
        }
    }
}
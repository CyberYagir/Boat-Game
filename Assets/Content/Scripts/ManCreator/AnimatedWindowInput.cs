using Content.Scripts.BoatGame.UI;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Content.Scripts.ManCreator
{
    public class AnimatedWindowInput : AnimatedWindow
    {
        [SerializeField] protected TMP_InputField inputField;
        [SerializeField] protected int charLimit = 8;
        private Color startTextColor;
        private float startScale;

        public override void ShowWindow()
        {
            base.ShowWindow();
            inputField.text = "";
            inputField.characterLimit = charLimit;
            startTextColor = inputField.textComponent.color;
            startScale = inputField.transform.localScale.x;
            inputField.ActivateInputField();
        }

        public virtual bool Apply()
        {
            var trim = inputField.text.Trim();

            if (trim.Length < 2 || trim.Length > charLimit)
            {
                ErrorAnimation();
                return false;
            }
            
            CloseWindow();

            return true;
        }
        
        public void ApplyButton()
        {
            Apply();
        }

        public void ErrorAnimation()
        {
            inputField.textComponent.DOKill();
            inputField.transform.DOKill();
            
            inputField.textComponent.color = startTextColor;
            inputField.textComponent.DOColor(Color.red, 0.15f).SetLoops(2, LoopType.Yoyo);
            inputField.transform.localScale = Vector3.one * startScale;
            inputField.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
        }

        public override void OnClosed()
        {
            base.OnClosed();
            
            gameObject.SetActive(false);
        }
    }
}
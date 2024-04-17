using Content.Scripts.BoatGame.UI;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Content.Scripts.ManCreator
{
    public class UIChangeNameWindow : AnimatedWindow
    {
        [SerializeField] private TMP_InputField inputField;
        private ManCreatorUIService manCreatorUIService;
        private Color startTextColor;
        private float startScale;
        public void Init(ManCreatorUIService manCreatorUIService)
        {
            this.manCreatorUIService = manCreatorUIService;
        }
        
        public override void ShowWindow()
        {
            base.ShowWindow();
            inputField.text = "";
            startTextColor = inputField.textComponent.color;
            startScale = inputField.transform.localScale.x;
            inputField.ActivateInputField();
        }

        public void Apply()
        {
            var trim = inputField.text.Trim();

            if (trim.Length is < 2 or > 8)
            {
                ErrorAnimation();
                return;
            }
            
            
            
            manCreatorUIService.ChangeName(inputField.text);
            
            CloseWindow();
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
